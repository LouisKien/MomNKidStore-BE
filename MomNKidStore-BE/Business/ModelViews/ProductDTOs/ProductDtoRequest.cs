using System.ComponentModel.DataAnnotations;

namespace MomNKidStore_BE.Business.ModelViews.ProductDTOs
{
    public class ProductDtoRequest
    {
        [Required]
        public int ProductCategoryId { get; set; }
        [Required]
        public string ProductName { get; set; } = null!;
        [Required]
        public string ProductInfor { get; set; } = null!;

        [Required]
        [Range(0, double.MaxValue)]
        public decimal ProductPrice { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int ProductQuantity { get; set; }
        //public bool ProductStatus { get; set; }

        public List<ImageProductDto> Images { get; set; } = new List<ImageProductDto>();
    }
}
