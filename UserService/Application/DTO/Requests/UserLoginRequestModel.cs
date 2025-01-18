using System.ComponentModel.DataAnnotations;

namespace UserService.Application.DTO.Requests
{
    public class UserLoginRequestModel
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
