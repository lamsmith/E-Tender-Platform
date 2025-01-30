namespace AuthService.Application.DTO.Requests
{
    public class ResetPasswordRequest
    {
        public Guid UserId { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}