using MomNKidStore_BE.Business.ModelViews.ProductDTOs;
using MomNKidStore_Repository.Entities;

namespace MomNKidStore_BE.Business.Services.Interfaces
{
    public interface IProductService
    {
        Task<bool> AddNewProduct(Product product, List<string> imagePaths);

        Task<(List<ProductDtoResponse> response, int totalPage)> GetAllProducts(int CategoryId, int page, int pageSize);

        Task<ProductDtoResponse> GetProductByID(int id);

        Task<bool> UpdateProduct(ProductDtoRequest request, List<string> imagePaths, int id);

        Task<List<ProductDtoResponse>> Search(string searchInput);

        Task<bool> UpdateProductStatus(int id, int status);

        Task<bool> HideProduct(int id);
    }
}
