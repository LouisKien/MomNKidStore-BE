using MomNKidStore_BE.Business.ModelViews.ProductDTOs;

namespace MomNKidStore_BE.Business.ModelViews.OrderDetailDTOs
{
    public class OrderDetailDtoResponse
    {
        public int OrderQuantity { get; set; }
        public double ProductPrice { get; set; }

        public ProductDtoResponse? product { get; set; }
    }
}
