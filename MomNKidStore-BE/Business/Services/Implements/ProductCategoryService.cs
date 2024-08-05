using AutoMapper;
using Azure.Core;
using MomNKidStore_BE.Business.ModelViews.ProductCategoryDTOs;
using MomNKidStore_BE.Business.Services.Interfaces;
using MomNKidStore_Repository.Entities;
using MomNKidStore_Repository.UnitOfWorks.Interfaces;

namespace MomNKidStore_BE.Business.Services.Implements
{
    public class ProductCategoryService : IProductCategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductCategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task CreateCategory(CategoryDtoRequest request)
        {
            try
            {
                var newCategory = new ProductCategory
                {
                    ProductCategoryName = request.ProductCategoryName,
                    ProductCategoryStatus = true
                };
                await _unitOfWork.ProductCategoryRepository.AddAsync(newCategory);
                await _unitOfWork.SaveAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task HideCategoryAndProduct(int CategoryId)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var category = (await _unitOfWork.ProductCategoryRepository.FindAsync(c => c.ProductCategoryId == CategoryId)).FirstOrDefault();
                    if (category == null)
                    {
                        throw new Exception("No category match this id");
                    }
                    else
                    {
                        category.ProductCategoryStatus = false;
                        await _unitOfWork.ProductCategoryRepository.UpdateAsync(category);

                        var products = await _unitOfWork.ProductRepository.GetAsync(filter: p => p.ProductCategoryId == category.ProductCategoryId);
                        if (products.Any())
                        {
                            foreach (var product in products)
                            {
                                product.ProductStatus = 2;
                                await _unitOfWork.ProductCategoryRepository.UpdateAsync(category);
                            }
                        }
                        await _unitOfWork.SaveAsync();
                        await transaction.CommitAsync();
                    }
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception(ex.Message);
                }
            }
        }

        public async Task<List<CategoryDtoResponse>> GetAllCategories()
        {
            try
            {
                var categoryes = await _unitOfWork.ProductCategoryRepository.GetAllAsync(c => c.ProductCategoryStatus == true);
                if (categoryes.Count() == 0)
                {
                    return null;
                }
                List<CategoryDtoResponse> categoryViews = new List<CategoryDtoResponse>();
                foreach (var Type in categoryes)
                {
                    var categoryView = _mapper.Map<CategoryDtoResponse>(Type);
                    categoryViews.Add(categoryView);
                }
                return categoryViews;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<CategoryDtoResponse> GetCategoryById(int id)
        {
            try
            {
                var category = (await _unitOfWork.ProductCategoryRepository.GetAllAsync(filter: c => c.ProductCategoryStatus == true && c.ProductCategoryId == id)).FirstOrDefault();
                if (category == null)
                {
                    return null;
                }
                var categoryView = _mapper.Map<CategoryDtoResponse>(category);
                return categoryView;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task UpdateCategory(int CategoryId, CategoryDtoRequest request)
        {
            try
            {
                var category = (await _unitOfWork.ProductCategoryRepository.FindAsync(c => c.ProductCategoryId == CategoryId)).FirstOrDefault();
                if (category == null)
                {
                    throw new Exception("No category match this id");
                }
                else
                {
                    category.ProductCategoryName = request.ProductCategoryName;
                    await _unitOfWork.ProductCategoryRepository.UpdateAsync(category);
                    await _unitOfWork.SaveAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
