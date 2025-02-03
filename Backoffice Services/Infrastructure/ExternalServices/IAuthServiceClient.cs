namespace Backoffice_Services.Infrastructure.ExternalServices
{
    public interface IAuthServiceClient
    {
        Task<UserDetailsDto> GetUserDetailsAsync(Guid userId);
        Task<List<UserDetailsDto>> GetPendingVerificationsAsync();
        Task<bool> UpdateUserVerificationStatusAsync(Guid userId, bool isApproved, string reason = null);
        Task<List<UserDetailsDto>> GetIncompleteOnboardingUsersAsync();
        Task<(Guid UserId, string TempPassword)> CreateStaffUserAsync(string email, string role);
        Task NotifyStaffUserAsync(Guid userId);
    }
}

public class UserDetailsDto
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public bool IsVerified { get; set; }
    public bool EmailConfirmed { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
}