using AuthService.Application.DTO.Requests;
using AuthService.Application.DTO.Responses;
using AuthService.Domain.Enums;
using SharedLibrary.Enums;

namespace AuthService.Application.Common.Interface.Services
{
    public interface IAuthService
    {
        //Task RegisterAsync(UserRegistrationRequestModel request);
        Task RegisterCorporateUserAsync(UserRegistrationRequestModel request);
        Task RegisterMSMEUserAsync(UserRegistrationRequestModel request);
        Task<UserLoginResponseModel> LoginAsync(UserLoginRequestModel request);
        Task LogoutAsync(Guid userId);
        Task<(Guid UserId, string TempPassword)> CreateStaffUserAsync(string email, Role role);
        Task SendStaffWelcomeEmailAsync(Guid userId);
        Task<bool> ResetPasswordAsync(Guid userId, string currentPassword, string newPassword);
        Task<bool> UpdateAccountStatusAsync(Guid userId, AccountStatus newStatus, string? reason);
    }
}