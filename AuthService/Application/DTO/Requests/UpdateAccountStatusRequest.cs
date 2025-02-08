using SharedLibrary.Enums;

namespace AuthService.Application.DTO.Requests
{
    public class UpdateAccountStatusRequest
    {
        public AccountStatus NewStatus { get; set; }
        public string? Reason { get; set; }
    }
}