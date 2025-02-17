using AuthService.Application.Common.Interface.Repositories;
using AuthService.Application.Common.Interface.Services;
using AuthService.Application.DTO.Requests;
using AuthService.Application.DTO.Responses;
using AuthService.Domain.Entities;
using AuthService.Domain.Enums;
using AuthService.Infrastructure.ExternalServices;
using AuthService.Infrastructure.JWT;
using MassTransit;
using SharedLibrary.Enums;
using SharedLibrary.Models.Messages;

namespace AuthService.Infrastructure.Services
{
    public class AuthServiceImpl : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IUserServiceClient _userServiceClient;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<AuthServiceImpl> _logger;

        public AuthServiceImpl(
            IUserRepository userRepository,
            IJwtTokenService jwtTokenService,
            IUserServiceClient userServiceClient,
            IPublishEndpoint publishEndpoint,
            ILogger<AuthServiceImpl> logger)
        {
            _userRepository = userRepository;
            _jwtTokenService = jwtTokenService;
            _userServiceClient = userServiceClient;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task<(Guid UserId, string Email)> RegisterCorporateUserAsync(UserRegistrationRequestModel request)
        {
            try
            {
                _logger.LogInformation("Registration attempt for email: {Email}", request.Email);

                if (await _userRepository.EmailExistsAsync(request.Email))
                {
                    _logger.LogWarning("Registration failed - Email already exists: {Email}", request.Email);
                    throw new Exception("Email already exists");
                }

                var authUser = new User
                {
                    Id = Guid.NewGuid(),
                    Email = request.Email,
                    PasswordHash = HashPassword(request.Password),
                    Role = Role.Corporate,
                    CreatedAt = DateTime.UtcNow,
                    EmailConfirmed = false,
                    IsActive = true,
                    Status = AccountStatus.Pending
                };

                await _userRepository.CreateAsync(authUser);

                // Publish the user creation event with profile data
                var createUserMessage = new CreateUserMessage
                {
                    UserId = authUser.Id,
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    CreatedAt = DateTime.UtcNow
                };

                _logger.LogInformation("Publishing CreateUserMessage for user {UserId} with email {Email}",
                    createUserMessage.UserId, createUserMessage.Email);

                await _publishEndpoint.Publish(createUserMessage);

                _logger.LogInformation("User successfully registered: {Email}", request.Email);

                return (authUser.Id, authUser.Email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration process for email: {Email}", request.Email);
                throw;
            }
        }

        public async Task<(Guid UserId, string Email)> RegisterMSMEUserAsync(UserRegistrationRequestModel request)
        {
            try
            {
                _logger.LogInformation("Registration attempt for email: {Email}", request.Email);

                if (await _userRepository.EmailExistsAsync(request.Email))
                {
                    _logger.LogWarning("Registration failed - Email already exists: {Email}", request.Email);
                    throw new Exception("Email already exists");
                }

                var authUser = new User
                {
                    Id = Guid.NewGuid(),
                    Email = request.Email,
                    PasswordHash = HashPassword(request.Password),
                    Role = Role.MSME,
                    CreatedAt = DateTime.UtcNow,
                    EmailConfirmed = false,
                    IsActive = true,
                    Status = AccountStatus.Pending
                };

                await _userRepository.CreateAsync(authUser);

                // Publish the user creation event with profile data
                var createUserMessage = new CreateUserMessage
                {
                    UserId = authUser.Id,
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    CreatedAt = DateTime.UtcNow
                };

                _logger.LogInformation("Publishing CreateUserMessage for user {UserId} with email {Email}",
                    createUserMessage.UserId, createUserMessage.Email);

                await _publishEndpoint.Publish(createUserMessage);

                _logger.LogInformation("User successfully registered: {Email}", request.Email);

                return (authUser.Id, authUser.Email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration process for email: {Email}", request.Email);
                throw;
            }
        }
        public async Task<(Guid UserId, string TempPassword)> CreateStaffUserAsync(string email)
        {
            try
            {
                if (await _userRepository.EmailExistsAsync(email))
                    throw new Exception("Email already exists");

                var tempPassword = GenerateTemporaryPassword();

                var user = new User
                {
                    Email = email,
                    PasswordHash = HashPassword(tempPassword),
                    Role = Role.Staff,
                    IsActive = true,
                    RequirePasswordChange = true,
                    CreatedAt = DateTime.UtcNow
                };

                var createdUser = await _userRepository.CreateAsync(user);

                return (createdUser.Id, tempPassword);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating staff user with email: {Email}", email);
                throw;
            }
        }

        public async Task SendStaffWelcomeEmailAsync(Guid userId)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                    throw new Exception("User not found");

                var message = new StaffWelcomeEmailMessage
                {
                    Email = user.Email
                };

                await _publishEndpoint.Publish(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending welcome email for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> ResetPasswordAsync(Guid userId, string currentPassword, string newPassword)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                    throw new Exception("User not found");

                if (!VerifyPassword(currentPassword, user.PasswordHash))
                    throw new Exception("Current password is incorrect");

                user.PasswordHash = HashPassword(newPassword);
                user.RequirePasswordChange = false;
                user.LastPasswordChangeAt = DateTime.UtcNow;

                await _userRepository.UpdateAsync(user);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<UserLoginResponseModel> LoginAsync(UserLoginRequestModel request)
        {
            try
            {
                _logger.LogInformation("Login attempt for user with email: {Email}", request.Email);

                var user = await _userRepository.GetByEmailAsync(request.Email);
                if (user == null || !VerifyPassword(request.Password, user.PasswordHash))
                {
                    _logger.LogWarning("Failed login attempt for email: {Email}", request.Email);
                    throw new Exception("Invalid email or password");
                }

                // Get user names from UserService
                //var userNames = await _userServiceClient.GetUserNamesAsync(user.Id);
                //if (userNames == null)
                //{
                //    _logger.LogWarning("Could not fetch user names for ID: {UserId}", user.Id);
                //    throw new Exception("Could not fetch user details");
                //}

                var token = _jwtTokenService.GenerateToken(user.Id, user.Email, user.Role.ToString());
                var refreshToken = _jwtTokenService.GenerateRefreshToken();
                var expiresAt = _jwtTokenService.GetExpirationDate();

                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
                user.LastLoginAt = DateTime.UtcNow;

                await _userRepository.UpdateAsync(user);

                _logger.LogInformation("User successfully logged in: {Email}", user.Email);

                return new UserLoginResponseModel
                {
                    Token = token,
                    RefreshToken = refreshToken,
                    ExpiresAt = expiresAt,
                    User = new UserDto
                    {
                        Id = user.Id,
                        Email = user.Email,
                        Role = user.Role.ToString(),
                        AccountStatus = user.Status,
                        //FirstName = userNames.FirstName,
                        //LastName = userNames.LastName
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login process for email: {Email}", request.Email);
                throw;
            }
        }


        public async Task LogoutAsync(Guid userId)
        {
            try
            {
                _logger.LogInformation("Logout attempt for user: {UserId}", userId);

                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("Logout failed - User not found: {UserId}", userId);
                    throw new Exception("User not found");
                }

                user.RefreshToken = null;
                user.RefreshTokenExpiryTime = null;

                await _userRepository.UpdateAsync(user);

                _logger.LogInformation("User successfully logged out: {UserId}", userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout process for user: {UserId}", userId);
                throw;
            }
        }

        private string GenerateTemporaryPassword(int length = 12)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }

        public async Task<bool> UpdateAccountStatusAsync(Guid userId, AccountStatus newStatus, string? reason)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                    return false;

                user.Status = newStatus;
                user.UpdatedAt = DateTime.UtcNow;

                await _userRepository.UpdateAsync(user);

                await _publishEndpoint.Publish(new UserStatusChangedMessage
                {
                    UserId = userId,
                    NewStatus = newStatus,
                    Reason = reason,
                    ChangedAt = DateTime.UtcNow
                });

                _logger.LogInformation(
                    "Account status updated for user {UserId} to {NewStatus}. Reason: {Reason}",
                    userId,
                    newStatus,
                    reason ?? "Not provided");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating account status for user {UserId}", userId);
                throw;
            }
        }
    }
}