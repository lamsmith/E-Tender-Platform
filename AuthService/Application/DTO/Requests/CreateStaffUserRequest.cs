using AuthService.Domain.Enums;

namespace AuthService.Application.DTO.Requests
{
    public class CreateStaffUserRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}