using System.ComponentModel.DataAnnotations;

namespace MomNKidStore_BE.Business.ModelViews.AccountDTOs
{
    public class StaffDtoRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;
        [Required]
        public string ConfirmPassword { get; set; } = null!;
    }
}
