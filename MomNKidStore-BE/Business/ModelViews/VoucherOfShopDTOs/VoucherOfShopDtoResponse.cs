namespace MomNKidStore_BE.Business.ModelViews.VoucherOfShopDTOs
{
    public class VoucherOfShopDtoResponse
    {
        public int VoucherId { get; set; }
        public double VoucherValue { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
    }
}
