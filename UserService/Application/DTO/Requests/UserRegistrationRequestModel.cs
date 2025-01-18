using System.ComponentModel.DataAnnotations;
using UserService.Domain.Enums;

namespace UserService.Application.DTO.Requests
{
    public class UserRegistrationRequestModel
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public Role Role { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

    }
}
