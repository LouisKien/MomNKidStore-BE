using AutoMapper;
using MomNKidStore_BE.Business.ModelViews.OrderDetailDTOs;
using MomNKidStore_BE.Business.ModelViews.OrderDTOs;
using MomNKidStore_BE.Business.ModelViews.ProductDTOs;
using MomNKidStore_BE.Business.Services.Interfaces;
using MomNKidStore_BE.Business.VNPay;
using MomNKidStore_Repository.Entities;
using MomNKidStore_Repository.UnitOfWorks.Interfaces;

namespace MomNKidStore_BE.Business.Services.Implements
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        public async Task<bool> CheckVoucher(int voucherId)
        {
            try
            {
                var voucher = await _unitOfWork.VoucherOfShopRepository.GetByIDAsync(voucherId);
                if (voucher == null)
                {
                    return false;
                }
                if (voucher.VoucherQuantity <= 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<string> CreateOrder(List<OrderProductDto> cartItems, int? voucherId, int exchangedPoint)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    decimal totalPrice = 0;
                    int customerId = cartItems[0].customerId;
                    List<OrderDetailDtoRequest> orderProducts = new List<OrderDetailDtoRequest>();
                    foreach (var cartItem in cartItems)
                    {
                        var product = await _unitOfWork.ProductRepository.GetByIDAsync(cartItem.productId);
                        totalPrice += (product.ProductPrice * cartItem.quantity);
                        var orderProduct = new OrderDetailDtoRequest
                        {
                            ProductId = product.ProductId,
                            ProductPrice = product.ProductPrice,
                            OrderQuantity = cartItem.quantity
                        };
                        orderProducts.Add(orderProduct);
                    }

                    // minus voucher quantity and discount if exist
                    if (voucherId != null)
                    {
                        var voucher = await _unitOfWork.VoucherOfShopRepository.GetByIDAsync(voucherId);
                        voucher.VoucherQuantity--;
                        await _unitOfWork.VoucherOfShopRepository.UpdateAsync(voucher);
                        await _unitOfWork.SaveAsync();

                        totalPrice -= (totalPrice * (decimal)voucher.VoucherValue / 100);
                    }

                    // minus point and discount if exchangedPoint > 0
                    if (exchangedPoint > 0)
                    {
                        if ((totalPrice / 2) < exchangedPoint)
                        {
                            exchangedPoint = (int)totalPrice / 2;
                        }
                        var customer = await _unitOfWork.CustomerRepository.GetByIDAsync(customerId);
                        customer.Point = customer.Point - exchangedPoint;
                        await _unitOfWork.CustomerRepository.UpdateAsync(customer);
                        await _unitOfWork.SaveAsync();

                        totalPrice -= exchangedPoint;
                    }

                    // create order
                    var order = new Order
                    {
                        CustomerId = customerId,
                        TotalPrice = totalPrice,
                        ExchangedPoint = exchangedPoint,
                        VoucherId = (voucherId == null ? null : voucherId),
                        Status = 0,
                        OrderDate = DateTime.Now,
                    };
                    await _unitOfWork.OrderRepository.AddAsync(order);
                    await _unitOfWork.SaveAsync();

                    // create order detail
                    foreach (var orderProduct in orderProducts)
                    {
                        var orderDetail = new OrderDetail
                        {
                            OrderId = order.OrderId,
                            ProductId = orderProduct.ProductId,
                            ProductPrice = (double)orderProduct.ProductPrice,
                            OrderQuantity = orderProduct.OrderQuantity,
                            Status = true
                        };
                        await _unitOfWork.OrderDetailRepository.AddAsync(orderDetail);
                        await _unitOfWork.SaveAsync();
                    }

                    // minus quantity of product
                    foreach (var orderProduct in orderProducts)
                    {
                        var product = await _unitOfWork.ProductRepository.GetByIDAsync(orderProduct.ProductId);
                        if (product.ProductQuantity < orderProduct.OrderQuantity)
                        {
                            throw new Exception("Not enough product in stock");
                        }
                        product.ProductQuantity = product.ProductQuantity - orderProduct.OrderQuantity;
                        await _unitOfWork.ProductRepository.UpdateAsync(product);
                        await _unitOfWork.SaveAsync();
                    }

                    // delete items in cart
                    foreach (var cartItem in cartItems)
                    {
                        var item = await _unitOfWork.CartRepository.GetByIDAsync(cartItem.cartId);
                        await _unitOfWork.CartRepository.DeleteAsync(item);
                        await _unitOfWork.SaveAsync();
                    }

                    var paymentUrl = CreateVnpayLink(order);
                    await transaction.CommitAsync();
                    return paymentUrl;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception(ex.Message);
                }
            }
        }

        public async Task<List<OrderDtoResponse>> Get()
        {
            try
            {
                var response = new List<OrderDtoResponse>();
                var orders = await _unitOfWork.OrderRepository.GetAsync();
                if (orders.Any())
                {
                    foreach (var order in orders)
                    {
                        var orderView = _mapper.Map<OrderDtoResponse>(order);
                        response.Add(orderView);
                    }
                }
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<OrderDtoResponse?> Get(int id)
        {
            try
            {
                var order = await _unitOfWork.OrderRepository.GetByIDAsync(id);
                if (order == null)
                {
                    return null;
                }
                var orderView = _mapper.Map<OrderDtoResponse>(order);
                var orderDetails = await _unitOfWork.OrderDetailRepository.GetAsync(od => od.OrderId == order.OrderId);
                foreach (var orderDetail in orderDetails)
                {
                    var odView = _mapper.Map<OrderDetailDtoResponse>(orderDetail);
                    var product = (await _unitOfWork.ProductRepository.GetAsync(p => p.ProductId == orderDetail.ProductId)).FirstOrDefault();
                    var productView = _mapper.Map<ProductDtoResponse>(product);
                    odView.product = _mapper.Map<ProductDtoResponse>(product);
                    var productImages = await _unitOfWork.ImageProductRepository.GetAsync(p => p.ProductId == product.ProductId);
                    if (productImages.Any())
                    {
                        foreach (var image in productImages)
                        {
                            var imageView = new ImageProductDto
                            {
                                ImageProduct1 = image.ImageProduct1
                            };
                            odView.product.Images.Add(imageView);
                        }
                    }
                    orderView.orderDetails.Add(odView);
                }
                return orderView;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<OrderDtoResponse>> GetByCustomerId(int customerId, int status)
        {
            try
            {
                var response = new List<OrderDtoResponse>();
                if (status == -1)
                {
                    var orders = await _unitOfWork.OrderRepository.GetAsync(o => o.CustomerId == customerId);
                    if (orders.Any())
                    {
                        foreach (var order in orders)
                        {
                            var orderView = _mapper.Map<OrderDtoResponse>(order);
                            response.Add(orderView);
                        }
                    }
                    return response;
                }
                else
                {
                    var orders = await _unitOfWork.OrderRepository.GetAsync(o => o.CustomerId == customerId && o.Status == status);
                    if (orders.Any())
                    {
                        foreach (var order in orders)
                        {
                            var orderView = _mapper.Map<OrderDtoResponse>(order);
                            response.Add(orderView);
                        }
                    }
                    return response;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> ValidateExchangedPoint(int exchangedPoint, int customerId)
        {
            try
            {
                var customer = await _unitOfWork.CustomerRepository.GetByIDAsync(customerId);
                if (customer == null)
                {
                    throw new Exception("Customer not found");
                }
                if (customer.Point < exchangedPoint)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> ValidateItemInCart(List<OrderProductDto> cartItems)
        {
            try
            {
                foreach (var cartItem in cartItems)
                {
                    var item = (await _unitOfWork.CartRepository.GetAsync(filter: c => c.CartId == cartItem.cartId, includeProperties: "Product")).FirstOrDefault();
                    if (item == null)
                    {
                        return -1;
                    }
                    else
                    {
                        if (item.Product.ProductQuantity == 0)
                        {
                            return -2;
                        }
                        else
                        {
                            if (cartItem.quantity > item.Product.ProductQuantity)
                            {
                                return -3;
                            }
                        }
                    }
                }
                return 0;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private string CreateVnpayLink(Order order)
        {
            var paymentUrl = string.Empty;

            var vpnRequest = new VNPayRequest(_configuration["VNpay:Version"], _configuration["VNpay:tmnCode"],
                order.OrderDate, "http://13.215.183.119:5173", (decimal)order.TotalPrice, "VND", "other",
                $"Thanh toan don hang {order.OrderId}", _configuration["VNpay:ReturnUrl"],
                $"{order.OrderId}");

            paymentUrl = vpnRequest.GetLink(_configuration["VNpay:PaymentUrl"],
                _configuration["VNpay:HashSecret"]);

            return paymentUrl;
        }
    }
}
