using System.ComponentModel.DataAnnotations;
using UserService.Domain.Enums;

namespace UserService.Application.DTO.Requests
{
    public class UserRegistrationRequestModel
    {
        [Required]
        public required string FirstName { get; set; }
        [Required]
        public required string LastName { get; set; }
        [Required]
        public Role Role { get; set; }
        [Required]
        public required string Email { get; set; }
        [Required]
        public required string Password { get; set; }
        [Required]
        [Compare("Password")]
        public required string ConfirmPassword { get; set; }

    }
}
