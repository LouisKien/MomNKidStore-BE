using AutoMapper;
using MomNKidStore_BE.Business.ModelViews.PaymentDTOs;
using MomNKidStore_BE.Business.Services.Interfaces;
using MomNKidStore_Repository.Entities;
using MomNKidStore_Repository.UnitOfWorks.Interfaces;
using System.Globalization;

namespace MomNKidStore_BE.Business.Services.Implements
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PaymentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PaymentDtoResponse> CancelTransaction(PaymentDtoRequest paymentRequest)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var existedOrder = await _unitOfWork.OrderRepository.GetByIDAsync(int.Parse(paymentRequest.vnp_TxnRef));
                    if (existedOrder != null)
                    {
                        var existedPayment = await _unitOfWork.PaymentRepository.GetAsync(p => p.OrderId == existedOrder.OrderId);
                        if (existedPayment.Any())
                        {
                            return null;
                        }
                        var payment = new Payment()
                        {
                            PaymentMethod = "VNPay",
                            BankCode = paymentRequest.vnp_BankCode,
                            BankTranNo = paymentRequest.vnp_BankTranNo,
                            CardType = paymentRequest.vnp_CardType,
                            PaymentInfo = paymentRequest.vnp_OrderInfo,
                            PayDate = DateTime.ParseExact(paymentRequest.vnp_PayDate, "yyyyMMddHHmmss", CultureInfo.InvariantCulture),
                            TransactionNo = paymentRequest.vnp_TransactionNo,
                            TransactionStatus = int.Parse(paymentRequest.vnp_TransactionStatus),
                            PaymentAmount = decimal.Parse(paymentRequest.vnp_Amount) / 100,
                            OrderId = int.Parse(paymentRequest.vnp_TxnRef)
                        };
                        await _unitOfWork.PaymentRepository.AddAsync(payment);

                        // Update Order's status is Cancelled
                        if (existedOrder.Status == 0)
                        {
                            existedOrder.Status = 2;

                            // return back quantity to product
                            var orderDetails = await _unitOfWork.OrderDetailRepository.GetAllAsync(o => o.OrderId == existedOrder.OrderId);
                            foreach (var od in orderDetails)
                            {
                                var product = await _unitOfWork.ProductRepository.GetByIDAsync(od.ProductId);
                                product.ProductQuantity += od.OrderQuantity;
                                await _unitOfWork.ProductRepository.UpdateAsync(product);
                            }
                        }
                        else if (existedOrder.Status == 10) {
                            existedOrder.Status = 12;
                        }
                        await _unitOfWork.OrderRepository.UpdateAsync(existedOrder);

                        // return points customer point if used
                        if (existedOrder.ExchangedPoint > 0)
                        {
                            var customer = await _unitOfWork.CustomerRepository.GetByIDAsync(existedOrder.CustomerId);
                            customer.Point += existedOrder.ExchangedPoint;
                            await _unitOfWork.CustomerRepository.UpdateAsync(customer);
                        }

                        // return voucher to shop if used
                        if (existedOrder.VoucherId != null)
                        {
                            var voucher = await _unitOfWork.VoucherOfShopRepository.GetByIDAsync(existedOrder.VoucherId);
                            voucher.VoucherQuantity++;
                            await _unitOfWork.VoucherOfShopRepository.UpdateAsync(voucher);
                        }

                        await _unitOfWork.SaveAsync();
                        await transaction.CommitAsync();
                        return _mapper.Map<PaymentDtoResponse>(payment);
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception(ex.Message);
                }
            }
        }

        public async Task<PaymentDtoResponse> CreatePayment(PaymentDtoRequest paymentRequest)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var existedOrder = await _unitOfWork.OrderRepository.GetByIDAsync(int.Parse(paymentRequest.vnp_TxnRef));
                    if (existedOrder != null)
                    {
                        var existedPayment = await _unitOfWork.PaymentRepository.GetAsync(p => p.OrderId == existedOrder.OrderId);
                        if (existedPayment.Any())
                        {
                            return null;
                        }
                        var payment = new Payment()
                        {
                            PaymentMethod = "VNPay",
                            BankCode = paymentRequest.vnp_BankCode,
                            BankTranNo = paymentRequest.vnp_BankTranNo,
                            CardType = paymentRequest.vnp_CardType,
                            PaymentInfo = paymentRequest.vnp_OrderInfo,
                            PayDate = DateTime.ParseExact(paymentRequest.vnp_PayDate, "yyyyMMddHHmmss", CultureInfo.InvariantCulture),
                            TransactionNo = paymentRequest.vnp_TransactionNo,
                            TransactionStatus = int.Parse(paymentRequest.vnp_TransactionStatus),
                            PaymentAmount = decimal.Parse(paymentRequest.vnp_Amount) / 100,
                            OrderId = int.Parse(paymentRequest.vnp_TxnRef)
                        };
                        await _unitOfWork.PaymentRepository.AddAsync(payment);

                        // Update Order's status is Paid
                        if (existedOrder.Status == 0)
                        {
                            existedOrder.Status = 1;
                        }
                        else if (existedOrder.Status == 10)
                        {
                            existedOrder.Status = 11;
                        }
                        await _unitOfWork.OrderRepository.UpdateAsync(existedOrder);

                        // accumulate points customer point
                        var customer = await _unitOfWork.CustomerRepository.GetByIDAsync(existedOrder.CustomerId);
                        customer.Point += (int)((double)payment.PaymentAmount * 0.02); // 2% per successful order
                        await _unitOfWork.CustomerRepository.UpdateAsync(customer);

                        await _unitOfWork.SaveAsync();
                        await transaction.CommitAsync();
                        return _mapper.Map<PaymentDtoResponse>(payment);
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception(ex.Message);
                }
            }
        }
    }
}
