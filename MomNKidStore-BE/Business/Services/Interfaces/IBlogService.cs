using MomNKidStore_BE.Business.ModelViews.BlogDTOs;

namespace MomNKidStore_BE.Business.Services.Interfaces
{
    public interface IBlogService
    {
        Task<IList<BlogDtoResponse>> GetAllBlog();

        Task<BlogDtoResponse> GetAllBlogByBlogId(int BlogId);
        Task<int> ValidateProductOfBlog(BlogProductDto blogItems);

        Task<string> CreateBlog(BlogProductDto blogItems);
        Task<string> UpdateBlog(int blogId, BlogProductDto blogItems);
        Task<bool> statusBlog(int blogId);

    }
}
