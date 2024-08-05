using System.ComponentModel.DataAnnotations;

namespace MomNKidStore_BE.Business.ModelViews.AccountDTOs
{
    public class UserRegisterDtoRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;
        [Required]
        public string ConfirmPassword { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public DateOnly? Dob { get; set; }
    }
}
