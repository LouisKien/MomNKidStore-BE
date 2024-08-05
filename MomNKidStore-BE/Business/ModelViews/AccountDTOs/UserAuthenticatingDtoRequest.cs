using System.ComponentModel.DataAnnotations;

namespace MomNKidStore_BE.Business.ModelViews.AccountDTOs
{
    public class UserAuthenticatingDtoRequest
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        public string? Password { get; set; }
    }
}
