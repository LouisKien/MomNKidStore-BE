namespace MomNKidStore_BE.Business.ModelViews.OrderDTOs
{
    public class OrderProductDto
    {
        public int cartId { get; set; }
        public int customerId { get; set; }
        public int productId { get; set; }
        public int quantity { get; set; }
    }
}
