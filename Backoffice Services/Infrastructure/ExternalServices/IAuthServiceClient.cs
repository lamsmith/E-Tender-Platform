namespace Backoffice_Services.Infrastructure.ExternalServices
{
    public interface IAuthServiceClient
    {
        Task<(Guid UserId, string TempPassword)> CreateStaffUserAsync(string email, string role);
        Task NotifyStaffUserAsync(Guid userId);
    }
}