using System.ComponentModel.DataAnnotations;

namespace MomNKidStore_BE.Business.ModelViews.OrderDTOs
{
    public class OrderProductInCartDto
    {
        public int cartId { get; set; }
        public int customerId { get; set; }
        public int productId { get; set; }
        [Range(1, int.MaxValue)]
        public int quantity { get; set; }
    }
}
