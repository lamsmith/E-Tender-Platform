using AuthService.Domain.Enums;

namespace AuthService.Application.DTO.Responses
{
    public class UserLoginResponseModel
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiresAt { get; set; }
        public UserDto User { get; set; }
    }

    public class UserDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public AccountStatus AccountStatus { get; set; }
    }
}