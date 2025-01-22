using System.ComponentModel.DataAnnotations;

namespace UserService.Application.DTO.Requests
{
    public class UserChangePasswordRequestModel
    {
        [Required]
        public required string CurrentPassword { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public required string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public required string ConfirmNewPassword { get; set; }
    }
}
