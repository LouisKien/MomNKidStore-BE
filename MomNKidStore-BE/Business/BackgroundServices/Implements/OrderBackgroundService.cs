using MomNKidStore_BE.Business.BackgroundServices.Interfaces;
using MomNKidStore_Repository.UnitOfWorks.Interfaces;

namespace MomNKidStore_BE.Business.BackgroundServices.Implements
{
    public class OrderBackgroundService : IOrderBackgroundService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderBackgroundService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task RejectExpiredOrder()
        {
            using (var Transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var orders = await _unitOfWork.OrderRepository.GetAllAsync(filter: o => o.OrderDate.AddDays(1) < DateTime.Now && o.Status == 0);
                    if (orders.Any())
                    {
                        foreach (var order in orders)
                        {
                            order.Status = 2;
                            await _unitOfWork.OrderRepository.UpdateAsync(order);

                            // return back quantity to product
                            var orderDetails = await _unitOfWork.OrderDetailRepository.GetAllAsync(o => o.OrderId == order.OrderId);
                            foreach (var od in orderDetails)
                            {
                                var product = await _unitOfWork.ProductRepository.GetByIDAsync(od.ProductId);
                                product.ProductQuantity += od.OrderQuantity;
                                await _unitOfWork.ProductRepository.UpdateAsync(product);
                                Console.WriteLine(product.ProductQuantity);
                            }

                            // return points customer point if used
                            if (order.ExchangedPoint > 0)
                            {
                                var customer = await _unitOfWork.CustomerRepository.GetByIDAsync(order.CustomerId);
                                customer.Point += order.ExchangedPoint;
                                await _unitOfWork.CustomerRepository.UpdateAsync(customer);
                                Console.WriteLine(customer.Point);
                            }

                            // return voucher to shop if used
                            if (order.VoucherId != null)
                            {
                                var voucher = await _unitOfWork.VoucherOfShopRepository.GetByIDAsync(order.VoucherId);
                                voucher.VoucherQuantity++;
                                await _unitOfWork.VoucherOfShopRepository.UpdateAsync(voucher);
                                Console.WriteLine(voucher.VoucherQuantity);
                            }
                            await _unitOfWork.SaveAsync();
                            await Transaction.CommitAsync();
                        }
                    }
                }
                catch (Exception ex)
                {
                    await Transaction.RollbackAsync();
                    Console.WriteLine(ex.ToString());
                }
            }
        }
    }
}
