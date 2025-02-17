using UserService.Application.DTO.Requests;
using UserService.Domain.Entities;
using UserService.Domain.Paging;

namespace UserService.Application.Common.Interface.Services
{
    public interface IUserService
    {
       
        Task<User> GetUserByIdAsync(Guid userId);
        Task<User> CompleteProfileAsync(Guid userId,CompleteProfileRequest request);
        Task<int> GetUserCountAsync();
        Task<User> SubmitKycAsync(Guid userId, KycSubmissionRequest request);
        Task<PaginatedList<User>> GetIncompleteProfilesAsync(PageRequest pageRequest);
        Task<(string FirstName, string LastName)?> GetUserNamesByIdAsync(Guid userId);
    }
}
