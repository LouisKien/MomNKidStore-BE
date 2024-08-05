namespace MomNKidStore_BE.Business.ModelViews.CustomerDTOs
{
    public class CustomerDto
    {
        public int CustomerId { get; set; }
        public string UserName { get; set; } = null!;
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public DateOnly? Dob { get; set; }
        public int? Point { get; set; }
    }
}
