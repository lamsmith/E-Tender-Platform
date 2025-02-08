using AuthService.Domain.Enums;


namespace AuthService.Application.DTO.Requests
{
    public class UserRegistrationRequestModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public Role Role { get; set; }
    }
}