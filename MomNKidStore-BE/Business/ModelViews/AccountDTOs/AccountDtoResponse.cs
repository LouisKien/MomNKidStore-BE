namespace MomNKidStore_BE.Business.ModelViews.AccountDTOs
{
    public class AccountDtoResponse
    {
        public string Email { get; set; } = null!;
        public int RoleId { get; set; }
        public string? userName { get; set; }
    }
}
