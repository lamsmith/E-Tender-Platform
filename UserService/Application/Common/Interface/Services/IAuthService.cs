using UserService.DTO.Requests;
using UserService.DTO.Responses;

namespace UserService.Application.Common.Interface.Services
{
    public interface IAuthService
    {
        Task RegisterAsync(UserRegistrationRequestModel request);
        Task<UserLoginResponseModel> LoginAsync(UserLoginRequestModel request);
        Task LogoutAsync();
    }
}
