namespace MomNKidStore_BE.Business.ModelViews.VoucherOfShopDTOs
{
    public class VoucherOfShopDtoResponseForAdmin
    {
        public int VoucherId { get; set; }
        public double VoucherValue { get; set; }
        public int VoucherQuantity { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool Status { get; set; }
    }
}
