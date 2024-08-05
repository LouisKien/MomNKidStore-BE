using MomNKidStore_BE.Business.ModelViews.ProductDTOs;

namespace MomNKidStore_BE.Business.ModelViews.CartDTOs
{
    public class CartDtoResponse
    {
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public int CustomerId { get; set; }
        public int CartQuantity { get; set; }

        public ProductDtoResponse ProductView { get; set; } = new ProductDtoResponse();
    }
}
