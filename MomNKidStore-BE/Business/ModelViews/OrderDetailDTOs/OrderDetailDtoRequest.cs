namespace MomNKidStore_BE.Business.ModelViews.OrderDetailDTOs
{
    public class OrderDetailDtoRequest
    {
        public int ProductId { get; set; }
        public int OrderQuantity { get; set; }
        public decimal ProductPrice { get; set; }
    }
}
