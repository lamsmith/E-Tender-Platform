using AuthService.Application.DTO.Requests;
using AuthService.Application.DTO.Responses;

namespace AuthService.Application.Common.Interface.Services
{
    public interface IAuthService
    {
        Task RegisterAsync(UserRegistrationRequestModel request);
        Task<UserLoginResponseModel> LoginAsync(UserLoginRequestModel request);
        Task LogoutAsync(Guid userId);
    }
}