namespace MomNKidStore_BE.Business.ModelViews.AccountDTOs
{
    public class UserAuthenticatingDtoResponse
    {
        public int AccountId { get; set; }
        public int RoleId { get; set; }
        public bool Status { get; set; }
    }
}
