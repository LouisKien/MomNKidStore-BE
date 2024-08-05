using MomNKidStore_BE.Business.BackgroundServices.Interfaces;
using MomNKidStore_Repository.UnitOfWorks.Interfaces;

namespace MomNKidStore_BE.Business.BackgroundServices.Implements
{
    public class ProductBackgroundService : IProductBackgroundService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductBackgroundService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task RemoveHiddenProductInCustomerCarts()
        {
            try
            {
                var hiddenProducts = await _unitOfWork.ProductRepository.GetAllAsync(p => p.ProductStatus == 0);
                if (hiddenProducts.Any())
                {
                    foreach (var product in hiddenProducts)
                    {
                        var productInCarts = await _unitOfWork.CartRepository.GetAllAsync(c => c.ProductId == product.ProductId);
                        if (productInCarts.Any())
                        {
                            await _unitOfWork.CartRepository.DeleteRangeAsync(productInCarts);
                            await _unitOfWork.SaveAsync();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
