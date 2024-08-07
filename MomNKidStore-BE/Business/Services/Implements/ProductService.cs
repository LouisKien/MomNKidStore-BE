using AutoMapper;
using MomNKidStore_BE.Business.ModelViews.ProductDTOs;
using MomNKidStore_BE.Business.Services.Interfaces;
using MomNKidStore_Repository.Entities;
using MomNKidStore_Repository.UnitOfWorks.Interfaces;

namespace MomNKidStore_BE.Business.Services.Implements
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;


        public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> AddNewProduct(Product product, List<string> imagePaths)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    bool status = false;
                    var checkCategory = await _unitOfWork.ProductCategoryRepository.GetByIDAsync(product.ProductCategoryId);
                    if (checkCategory != null)
                    {
                        if(checkCategory.ProductCategoryStatus == true)
                        {
                            product.ProductStatus = 1;
                            await _unitOfWork.ProductRepository.AddAsync(product);
                            await _unitOfWork.SaveAsync();

                            if (imagePaths.Any())
                            {
                                foreach (var imagePath in imagePaths)
                                {
                                    if (!String.IsNullOrEmpty(imagePath))
                                    {
                                        var image = new ImageProduct
                                        {
                                            ProductId = product.ProductId,
                                            ImageProduct1 = imagePath
                                        };
                                        await _unitOfWork.ImageProductRepository.AddAsync(image);
                                        await _unitOfWork.SaveAsync();
                                    }
                                }
                            }

                            status = true;
                            await transaction.CommitAsync();
                            return status;
                        } else
                        {
                            return status;
                        }
                    }
                    else
                    {
                        return status;
                    }

                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception(ex.Message);
                }
            }
        }

        public async Task<bool> UpdateProductStatus(int id, int status)
        {
            try
            {
                var checkProduct = await _unitOfWork.ProductRepository.GetByIDAsync(id);
                if (checkProduct != null)
                {
                    checkProduct.ProductStatus = status;
                    await _unitOfWork.ProductRepository.UpdateAsync(checkProduct);
                    await _unitOfWork.SaveAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<ProductDtoResponse>> GetAllProducts(int CategoryId, int page, int pageSize)
        {
            try
            {
                var products = new List<Product>();
                if (CategoryId == 0)
                {
                    products = (await _unitOfWork.ProductRepository.GetAsync(filter: p => p.ProductStatus == 1, pageIndex: page, pageSize: pageSize)).ToList();
                }
                else
                {
                    products = (await _unitOfWork.ProductRepository.GetAsync(filter: p => p.ProductStatus == 1 && p.ProductCategoryId == CategoryId, pageIndex: page, pageSize: pageSize)).ToList();
                }
                if (products.Any())
                {
                    List<ProductDtoResponse> list = new List<ProductDtoResponse>();
                    foreach (var product in products)
                    {
                        var productView = _mapper.Map<ProductDtoResponse>(product);
                        var productImages = (await _unitOfWork.ImageProductRepository.GetAsync(p => p.ProductId == product.ProductId)).FirstOrDefault();
                        if (productImages != null)
                        {
                            var image = new ImageProductDto
                            {
                                ImageProduct1 = productImages.ImageProduct1
                            };
                            productView.Images.Add(image);
                        }
                        list.Add(productView);
                    }
                    return list;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public async Task<ProductDtoResponse> GetProductByID(int id)
        {
            try
            {
                var product = (await _unitOfWork.ProductRepository.GetAsync(filter: p => p.ProductId == id && p.ProductStatus == 1, includeProperties: "ProductCategory")).FirstOrDefault();
                if (product != null)
                {
                    var productView = _mapper.Map<ProductDtoResponse>(product);
                    var productImages = await _unitOfWork.ImageProductRepository.GetAsync(p => p.ProductId == product.ProductId);
                    if (productImages.Any())
                    {
                        foreach (var image in productImages)
                        {
                            var imageView = new ImageProductDto
                            {
                                ImageProduct1 = image.ImageProduct1
                            };
                            productView.Images.Add(imageView);
                        }
                    }
                    return productView;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<ProductDtoResponse>> Search(string searchInput)
        {
            try
            {
                var products = (await _unitOfWork.ProductRepository.FindAsync(p => searchInput != null && p.ProductStatus == 1 && (p.ProductName.Contains(searchInput) || (p.ProductInfor.Contains(searchInput))))).ToList();
                if (products.Any())
                {
                    List<ProductDtoResponse> list = new List<ProductDtoResponse>();
                    foreach (var product in products)
                    {
                        var productView = _mapper.Map<ProductDtoResponse>(product);
                        var productImages = (await _unitOfWork.ImageProductRepository.GetAsync(p => p.ProductId == product.ProductId)).FirstOrDefault();
                        if (productImages != null)
                        {
                            var imageView = new ImageProductDto
                            {
                                ImageProduct1 = productImages.ImageProduct1
                            };
                            productView.Images.Add(imageView);
                        }
                        list.Add(productView);
                    }
                    return list;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }


        public async Task<(bool check, List<string>? oldImagePaths)> UpdateProduct(ProductDtoRequest request, List<string> imagePaths, int id)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    bool status = false;
                    var checkCategory = _unitOfWork.ProductCategoryRepository.GetByIDAsync(request.ProductCategoryId);
                    if (checkCategory != null)
                    {
                        var checkProduct = await _unitOfWork.ProductRepository.GetByIDAsync(id);
                        if (checkProduct != null)
                        {
                            var product = _mapper.Map(request, checkProduct);
                            await _unitOfWork.ProductRepository.UpdateAsync(product);
                            await _unitOfWork.SaveAsync();
                            var currentImagePaths = new List<string>();

                            var currentImages = await _unitOfWork.ImageProductRepository.GetAsync(p => p.ProductId == checkProduct.ProductId);
                            if (currentImages.Any())
                            {
                                foreach (var image in currentImages)
                                {
                                    await _unitOfWork.ImageProductRepository.DeleteAsync(image);
                                    await _unitOfWork.SaveAsync();
                                    currentImagePaths.Add(image.ImageProduct1);
                                }
                            }

                            if (imagePaths.Any())
                            {
                                foreach (var imagePath in imagePaths)
                                {
                                    if (!String.IsNullOrEmpty(imagePath))
                                    {
                                        var image = new ImageProduct
                                        {
                                            ProductId = checkProduct.ProductId,
                                            ImageProduct1 = imagePath
                                        };
                                        await _unitOfWork.ImageProductRepository.AddAsync(image);
                                        await _unitOfWork.SaveAsync();
                                    }
                                }
                            }

                            status = true;
                            await transaction.CommitAsync();
                            return (status, currentImagePaths);
                        }
                        else
                        {
                            return (status, null);
                        }
                    }
                    else
                    {
                        return (status, null);
                    }

                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception(ex.Message);
                }
            }
        }

        public async Task<bool> HideProduct(int id)
        {
            try
            {
                var checkProduct = await _unitOfWork.ProductRepository.GetByIDAsync(id);
                if (checkProduct != null)
                {
                    checkProduct.ProductStatus = 2;
                    await _unitOfWork.ProductRepository.UpdateAsync(checkProduct);
                    await _unitOfWork.SaveAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
