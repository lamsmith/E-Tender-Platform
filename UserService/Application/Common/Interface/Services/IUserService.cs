using UserService.Application.DTO.Requests;
using UserService.Domain.Entities;

namespace UserService.Application.Common.Interface.Services
{
    public interface IUserService
    {
        Task<User> UpdateProfileAsync(Guid userId, CompleteProfileRequest request);
        Task<User> GetUserByIdAsync(Guid userId);
        Task<User> UpdateProfileAsync(Guid userId, UserProfileUpdateRequestModel profileUpdate);
        Task<User> CompleteProfileAsync(Guid userId, CompleteProfileRequest request);


        Task<int> GetUserCountAsync();
    }
}
