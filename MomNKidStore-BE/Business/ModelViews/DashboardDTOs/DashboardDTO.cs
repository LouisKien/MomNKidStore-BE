using MomNKidStore_BE.Business.ModelViews.ProductDTOs;

namespace MomNKidStore_BE.Business.ModelViews.DashboardDTOs
{
    public class DashboardDTO
    {
        public int totalSoldProduct { get; set; } = 0;
        public decimal totalRevenue { get; set; } = 0;
        public int totalOrder { get; set; } = 0;
        public List<ProductDtoResponse> topSellingProducts { get; set; } = new List<ProductDtoResponse>();
    }
}
