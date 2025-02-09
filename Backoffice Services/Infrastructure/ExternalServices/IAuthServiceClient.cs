namespace Backoffice_Services.Infrastructure.ExternalServices
{
    public interface IAuthServiceClient
    {
        Task<AuthUserDto> GetUserDetailsAsync(Guid userId);
        Task<List<PendingVerificationDto>> GetPendingVerificationsAsync();
        Task<bool> UpdateUserVerificationStatusAsync(Guid userId, bool isApproved, string reason = null);
        Task<(Guid UserId, string TempPassword)> CreateStaffUserAsync(string email, string role);
        Task NotifyStaffUserAsync(Guid userId);
    }

    public class AuthUserDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public bool IsVerified { get; set; }
        public bool EmailConfirmed { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
    }

    public class PendingVerificationDto
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

