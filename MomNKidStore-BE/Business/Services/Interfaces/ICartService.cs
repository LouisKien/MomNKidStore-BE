using MomNKidStore_BE.Business.ModelViews.CartDTOs;

namespace MomNKidStore_BE.Business.Services.Interfaces
{
    public interface ICartService
    {
        Task<bool> AddToCart(CartDtoRequest request);

        Task<List<CartDtoResponse>> GetCartByCustomerId(int CustomerId);

        Task<bool> DeleteItemInCart(int id);

        Task<int> UpdateItemQuantityInCart(int id, int quantity);
    }
}
