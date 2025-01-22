using System.ComponentModel.DataAnnotations;

namespace UserService.Application.DTO.Requests
{
    public class UserLoginRequestModel
    {
        [Required]
        public required string Email { get; set; }
        [Required]
        public required string Password { get; set; }
    }
}
