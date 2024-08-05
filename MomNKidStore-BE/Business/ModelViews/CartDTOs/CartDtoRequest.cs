namespace MomNKidStore_BE.Business.ModelViews.CartDTOs
{
    public class CartDtoRequest
    {
        public int ProductId { get; set; }
        public int CustomerId { get; set; }
        public int CartQuantity { get; set; }
    }
}
