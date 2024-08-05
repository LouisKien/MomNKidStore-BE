namespace MomNKidStore_BE.Business.ModelViews.VoucherOfShopDTOs
{
    public class VoucherOfShopDtoResponseForAdmin
    {
        public int VoucherId { get; set; }
        public double VoucherValue { get; set; }
        public int VoucherQuantity { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public bool Status { get; set; }
    }
}
