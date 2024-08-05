using System.ComponentModel.DataAnnotations;

namespace MomNKidStore_BE.Business.ModelViews.VoucherOfShopDTOs
{
    public class VoucherOfShopDtoRequest
    {
        [Range(0, 100)]
        public double VoucherValue { get; set; }
        [Range(0, int.MaxValue)]
        public int VoucherQuantity { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
    }
}
