namespace MomNKidStore_BE.Business.ModelViews.ProductDTOs
{
    public class ProductDtoResponse
    {
        public int ProductId { get; set; }
        public int ProductCategoryId { get; set; }
        public string ProductName { get; set; } = null!;
        public string ProductInfor { get; set; } = null!;
        public double ProductPrice { get; set; }
        public int ProductQuatity { get; set; }
        public bool ProductStatus { get; set; }
        public List<ImageProductDto> Images { get; set; } = new List<ImageProductDto>();
        public CategoryDto? category { get; set; }
    }
}
