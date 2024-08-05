namespace MomNKidStore_BE.Business.ModelViews.VoucherOfShopDTOs
{
    public class VoucherOfShopDtoResponse
    {
        public int VoucherId { get; set; }
        public double VoucherValue { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
