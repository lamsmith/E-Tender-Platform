using SharedLibrary.Models.Messages;
using System.Security.Cryptography;
using System.Text;
using UserService.Application.Common.Interface.Repositories;
using UserService.Application.Common.Interface.Services;
using UserService.Application.DTO.Requests;
using UserService.Domain.Entities;
using UserService.Domain.Paging;
using Microsoft.Extensions.Logging;

namespace UserService.Infrastructure.Services
{
    public class UserServices : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserServices> _logger;

        public UserServices(IUserRepository userRepository, ILogger<UserServices> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<User> CompleteProfileAsync(Guid userId, CompleteProfileRequest request)
        {
            try
            {
                if (request == null)
                {
                    throw new ArgumentNullException(nameof(request), "Profile request cannot be null.");
                }

                if (string.IsNullOrEmpty(request.Email))
                {
                    throw new ArgumentException("Email cannot be null or empty", nameof(request));
                }

                _logger.LogInformation("Completing profile for user {UserId} with email {Email}", userId, request.Email);

                // Check if user already exists
                var existingUser = await _userRepository.GetByIdAsync(userId);
                if (existingUser != null && existingUser.Profile != null)
                {
                    _logger.LogWarning("Profile already exists for user {UserId}", userId);
                    throw new InvalidOperationException("User profile already exists.");
                }

                // Create new user if it doesn't exist, or update existing one
                var user = existingUser ?? new User();

                // Update user properties
                user.Id = Guid.NewGuid();
                user.UserId = userId;
                user.Email = request.Email;  
                user.CreatedAt = DateTime.UtcNow;
                user.CreatedBy = userId;

                // Create or update profile
            user.Profile = new Profile
            {
                UserId = userId,
                FirstName = request.FirstName,
                LastName = request.LastName,
                    CompanyName = null,
                    PhoneNumber = null,
                    CompanyAddress = null,
                    RcNumber = null,
                    State = null,
                    City = null,
                    Industry = null,
                    CreatedAt = DateTime.UtcNow
                };

                if (existingUser == null)
                {
                    _logger.LogInformation("Adding new user with ID {UserId}", userId);
                    await _userRepository.AddAsync(user);
                }
                else
                {
                    _logger.LogInformation("Updating existing user with ID {UserId}", userId);
            await _userRepository.UpdateAsync(user);
                }

                _logger.LogInformation("Successfully completed profile for user {UserId}", userId);
            return user;
        }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing profile for user {UserId}", userId);
                throw;
            }
        }

        public async Task<User> GetUserByIdAsync(Guid userId)
        {
            return await _userRepository.GetByIdAsync(userId);
        }

        public async Task<int> GetUserCountAsync() => await _userRepository.GetCountAsync();

        public async Task<User> SubmitKycAsync(Guid userId, KycSubmissionRequest request)
        {
            var user = await GetUserByIdAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException($"User not found with ID: {userId}");
            }

            // Handle company logo upload if provided
            if (request.CompanyLogo != null)
            {
                // TODO: Implement file upload logic
                var companyLogo = new CompanyLogo
                {
                    FileName = request.CompanyLogo.FileName,
                    FileType = request.CompanyLogo.ContentType
                    // Set FileUrl after upload
                };
                user.Profile.CompanyLogo = companyLogo;
            }

            // Update KYC information using existing Profile fields
            user.Profile.CompanyName = request.CompanyName;
            user.Profile.PhoneNumber = request.PhoneNumber;
            user.Profile.CompanyAddress = request.CompanyAddress;
            user.Profile.Industry = request.Industry;
            user.Profile.State = request.State;
            user.Profile.City = request.City;

            await _userRepository.UpdateAsync(user);

            return user;
        }

        public async Task<PaginatedList<User>> GetIncompleteProfilesAsync(PageRequest pageRequest)
        {
            return await _userRepository.GetIncompleteProfilesAsync(pageRequest);
        }

        public async Task<(string FirstName, string LastName)?> GetUserNamesByIdAsync(Guid userId)
        {
            return await _userRepository.GetUserNamesByIdAsync(userId);
        }
    }
}
