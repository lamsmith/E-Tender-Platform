namespace Backoffice_Services.Infrastructure.ExternalServices
{
    public interface IUserProfileServiceClient
    {
        Task<UserProfileDto> GetUserProfileAsync(Guid userId);
        Task<List<UserProfileDto>> GetUserProfilesAsync(IEnumerable<Guid> userIds);
        Task<UserProfileDto> GetUserDetailsAsync(Guid userId); 
        Task<List<PendingVerificationDto>> GetPendingVerificationsAsync();
    }

    public class UserProfileDto 
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public bool IsVerified { get; set; }
        public DateTime CreatedAt { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CompanyName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Industry { get; set; }
        public DateTime ProfileCompletedAt { get; set; }
    }

    public class PendingVerificationDto
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
