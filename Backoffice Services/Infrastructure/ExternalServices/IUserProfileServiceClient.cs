using Backoffice_Services.Application.DTO.UserManagement.Responses;
using Backoffice_Services.Domain.Paging;

namespace Backoffice_Services.Infrastructure.ExternalServices
{
    public interface IUserProfileServiceClient
    {
        Task<UserDetailsResponseModel> GetUserProfileAsync(Guid userId);
        Task<List<UserDetailsResponseModel>> GetUserProfilesAsync(IEnumerable<Guid> userIds);
        Task<UserDetailsResponseModel> GetUserDetailsAsync(Guid userId);
        Task<List<PendingVerificationResponseModel>> GetPendingVerificationsAsync();
        Task<PaginatedList<UserDetailsResponseModel>> GetUsersWithIncompleteProfilesAsync(PageRequest pageRequest);
    }
}
