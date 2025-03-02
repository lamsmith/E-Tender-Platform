namespace Backoffice_Services.Infrastructure.ExternalServices
{
    public interface IAuthServiceClient
    {
        Task<bool> UpdateUserVerificationStatusAsync(Guid userId, bool isApproved, string reason = null);
        Task<(Guid UserId, string TempPassword)> CreateStaffUserAsync(string email, string firstName, string lastName);
        Task<bool> UpdateAccountStatusAsync(Guid userId, SharedLibrary.Enums.AccountStatus newStatus, string? reason = null);
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
}

