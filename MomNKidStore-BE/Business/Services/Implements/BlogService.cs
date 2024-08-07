using AutoMapper;
using MomNKidStore_BE.Business.ModelViews.BlogDTOs;
using MomNKidStore_BE.Business.ModelViews.BlogProductDTOs;
using MomNKidStore_BE.Business.ModelViews.ProductDTOs;
using MomNKidStore_BE.Business.Services.Interfaces;
using MomNKidStore_Repository.Entities;
using MomNKidStore_Repository.UnitOfWorks.Interfaces;

namespace MomNKidStore_BE.Business.Services.Implements
{
    public class BlogService : IBlogService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;


        public BlogService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;

        }

        public async Task<String> CreateBlog(BlogProductDto blogItems)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    //int productId = blogItems.productId[0];
                    List<BlogProductDtoRequest> blogProducts = new List<BlogProductDtoRequest>();
                    foreach (var blogItem in blogItems.productId)
                    {
                        var product = await _unitOfWork.ProductRepository.GetByIDAsync(blogItem);
                        if (product != null)
                        {
                            var blogProduct = new BlogProductDtoRequest
                            {
                                ProductId = product.ProductId,
                            };
                            blogProducts.Add(blogProduct);
                        }
                        
                    }
                    // create blog
                    var blog = _mapper.Map<Blog>(blogItems);

                    await _unitOfWork.BlogRepository.AddAsync(blog);
                    // create blog product
                    foreach (var blogProduct in blogProducts)
                    {
                        var blogDetail = new BlogProduct
                        {
                            BlogId = blog.BlogId,
                            ProductId = blogProduct.ProductId,
                            Status = true
                        };
                        await _unitOfWork.BlogProductRepository.AddAsync(blogDetail);
                    }
                    await transaction.CommitAsync();
                    return "Blog created successfully.";

                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception(ex.Message);
                }
            }
        }

        public async Task<IList<BlogDtoResponse>> GetAllBlog()
        {
            try
            {
                var blogs = await _unitOfWork.BlogRepository.GetAllAsync(c => c.Status == true);
                if (blogs.Count() == 0)
                {
                    return null;
                }
                List<BlogDtoResponse> blogViews = new List<BlogDtoResponse>();
                foreach (var Type in blogs)
                {
                    var blogView = _mapper.Map<BlogDtoResponse>(Type);
                    blogViews.Add(blogView);
                }
                return blogViews;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<BlogDtoResponse> GetAllBlogByBlogId(int blogId)
        {


            var blog = await _unitOfWork.BlogRepository
                               .SingleOrDefaultAsync(x => x.BlogId == blogId && x.Status == true);

            if (blog == null)
                return null;

            var blogProducts = await _unitOfWork.BlogProductRepository
                                                .GetAllAsync(bp => bp.BlogId == blogId);
            var productIds = blogProducts.Select(bp => bp.ProductId).ToList();

            var products = await _unitOfWork.ProductRepository
                                            .GetAllAsync(p => productIds.Contains(p.ProductId));

            var response = new BlogDtoResponse
            {
                BlogId = blog.BlogId,
                BlogTitle = blog.BlogTitle,
                BlogContent = blog.BlogContent,
                BlogImage = blog.BlogImage,
                BlogProducts = products.Select(p => new ProductDtoResponse
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    ProductPrice = (double)p.ProductPrice
                }).ToList(),
                Status = blog.Status
            };

            return (BlogDtoResponse)response;
        }

        public async Task<bool> statusBlog(int blogId)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var blogExist = await _unitOfWork.BlogRepository.GetByIDAsync(blogId);
                    if (blogExist != null)
                    {
                        if (blogExist.Status == true)
                        {
                            blogExist.Status = false;
                            await _unitOfWork.BlogRepository.UpdateAsync(blogExist);
                            await transaction.CommitAsync();
                            return true;
                        }
                        else
                        {
                            blogExist.Status = true;
                            await _unitOfWork.BlogRepository.UpdateAsync(blogExist);
                            await transaction.CommitAsync();
                            return true;
                        }
                    }
                    return false;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception(ex.Message);
                }
            }
            
        }

        public async Task<string> UpdateBlog(int blogId, BlogProductDto blogItems)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    //int productId = blogItems.productId[0];
                    var blogExist = await _unitOfWork.BlogRepository.GetByIDAsync(blogId);
                    if (blogExist != null)
                    {
                        List<BlogProductDtoRequest> blogProducts = new List<BlogProductDtoRequest>();
                        foreach (var blogItem in blogItems.productId)
                        {
                            var product = await _unitOfWork.ProductRepository.GetByIDAsync(blogItem);
                            if (product != null)
                            {
                                var blogProduct = new BlogProductDtoRequest
                                {
                                    ProductId = product.ProductId,
                                };
                                blogProducts.Add(blogProduct);
                            }

                        }
                        // update blog
                        var blog = _mapper.Map(blogItems, blogExist);

                        await _unitOfWork.BlogRepository.UpdateAsync(blog);
                        // update blog product
                        var oldBlogDetail = await _unitOfWork.BlogProductRepository.FindAsync(b => b.BlogId == blogId);
                        if(oldBlogDetail != null)
                        {
                            foreach (var product in oldBlogDetail)
                            {
                                await _unitOfWork.BlogProductRepository.DeleteAsync(product);

                            }
                        }
                        else
                        {
                            return "Blog Product in Blog not found.";
                        }
                        

                        foreach (var blogProduct in blogProducts)
                        {
                             var blogDetail = new BlogProduct
                             {
                                 BlogId = blog.BlogId,
                                 ProductId = blogProduct.ProductId,
                                 Status = true
                             };
                             await _unitOfWork.BlogProductRepository.AddAsync(blogDetail);
                        }
                        
                        
                        await transaction.CommitAsync();
                        return "Blog created successfully.";
                    }
                    else
                    {
                        return "Blog not found.";
                    }
                    

                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception(ex.Message);
                }
            }
        }

        public async Task<int> ValidateProductOfBlog(BlogProductDto blogItems)
        {
            try
            {
                foreach (var blogItem in blogItems.productId)
                {
                    var item = (await _unitOfWork.ProductRepository.GetAsync(filter: c => c.ProductId == blogItem)).FirstOrDefault();
                    if (item == null)
                    {
                        return -1;
                    }
                }
                return 0;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
