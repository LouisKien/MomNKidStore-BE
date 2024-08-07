using MomNKidStore_BE.Business.ModelViews.OrderDTOs;

namespace MomNKidStore_BE.Business.Services.Interfaces
{
    public interface IOrderService
    {
        Task<int> ValidateItemInCart(List<OrderProductDto> cartItems);
        Task<string> CreateOrder(List<OrderProductDto> cartItems, int? voucherId, int exchangedPoint);
        Task<bool> CheckVoucher(int voucherId);
        Task<bool> ValidateExchangedPoint(int exchangedPoint, int customerId);
        Task<List<OrderDtoResponse>> Get();
        Task<OrderDtoResponse?> Get(int id);
        Task<List<OrderDtoResponse>> GetByCustomerId(int customerId, int status);
        Task<bool> UpdateOrderStatus(int id, int status);
    }
}
