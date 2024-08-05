using MomNKidStore_BE.Business.ModelViews.ProductCategoryDTOs;

namespace MomNKidStore_BE.Business.Services.Interfaces
{
    public interface IProductCategoryService
    {
        Task<List<CategoryDtoResponse>> GetAllCategories();
        Task<CategoryDtoResponse> GetCategoryById(int id);
        Task CreateCategory(CategoryDtoRequest request);
        Task UpdateCategory(int CategoryId, CategoryDtoRequest request);
        Task HideCategoryAndProduct(int CategoryId);
    }
}
