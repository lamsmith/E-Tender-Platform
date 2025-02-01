using AuthService.Application.Common.Interface.Repositories;
using AuthService.Application.Common.Interface.Services;
using AuthService.Application.DTO.Requests;
using AuthService.Application.DTO.Responses;
using AuthService.Domain.Entities;
using AuthService.Domain.Enums;
using AuthService.Infrastructure.ExternalServices;
using AuthService.Infrastructure.JWT;
using SharedLibrary.Constants;
using SharedLibrary.MessageBroker;
using SharedLibrary.Models.Messages;
using System.Security.Cryptography;
using System.Text;

namespace AuthService.Infrastructure.Services
{
    public class AuthServiceImpl : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IUserServiceClient _userServiceClient;
        private readonly IMessagePublisher _messagePublisher;
        private readonly ILogger<AuthServiceImpl> _logger;

        public AuthServiceImpl(
            IUserRepository userRepository,
            IJwtTokenService jwtTokenService,
            IUserServiceClient userServiceClient,
            IMessagePublisher messagePublisher,
            ILogger<AuthServiceImpl> logger)
        {
            _userRepository = userRepository;
            _jwtTokenService = jwtTokenService;
            _userServiceClient = userServiceClient;
            _messagePublisher = messagePublisher;
            _logger = logger;
        }

        public async Task<(Guid UserId, string TempPassword)> CreateStaffUserAsync(string email, Role role)
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
                    Role = role,
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

                _messagePublisher.PublishMessage(MessageQueues.Notifications, message);
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

                var userDetails = await _userServiceClient.GetUserDetailsAsync(user.Id);
                if (!userDetails.IsActive)
                {
                    _logger.LogWarning("Login attempt for inactive account: {Email}", request.Email);
                    throw new Exception("User account is inactive");
                }

                var token = _jwtTokenService.GenerateToken(user.Id, user.Email, userDetails.Role);
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
                        Role = userDetails.Role
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login process for email: {Email}", request.Email);
                throw;
            }
        }

        public async Task RegisterAsync(UserRegistrationRequestModel request)
        {
            try
            {
                _logger.LogInformation("Registration attempt for email: {Email}", request.Email);

                if (await _userRepository.EmailExistsAsync(request.Email))
                {
                    _logger.LogWarning("Registration failed - Email already exists: {Email}", request.Email);
                    throw new Exception("Email already exists");
                }


                if (request.SelectedRole == Role.Admin || request.SelectedRole == Role.Staff)
                {
                    _logger.LogWarning("Registration failed -Cannot register as Admin or Staff.");
                    throw new Exception("Cannot register as Admin or Staff.");
                }



                var authUser = new User
                {
                    Id = Guid.NewGuid(),
                    Email = request.Email,
                    PasswordHash = HashPassword(request.Password),
                    Role = request.SelectedRole,
                    CreatedAt = DateTime.UtcNow,
                    EmailConfirmed = false,
                    IsActive = true
                };

                await _userRepository.CreateAsync(authUser);

                var createUserMessage = new CreateUserMessage
                {
                    UserId = authUser.Id,
                    Email = authUser.Email,
                    CreatedAt = authUser.CreatedAt
                };

                _messagePublisher.PublishMessage(MessageQueues.UserCreated, createUserMessage);

                _logger.LogInformation("User successfully registered: {Email}", request.Email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration process for email: {Email}", request.Email);
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
    }
}