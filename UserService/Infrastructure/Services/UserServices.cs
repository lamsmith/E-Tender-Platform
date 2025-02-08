using System.Security.Cryptography;
using System.Text;
using UserService.Application.Common.Interface.Repositories;
using UserService.Application.Common.Interface.Services;
using UserService.Application.DTO.Requests;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.Services
{
    public class UserServices : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserServices(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        private bool VerifyPassword(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (string.IsNullOrEmpty(password)) throw new ArgumentException("Password cannot be null or empty.", nameof(password));
            if (storedHash == null || storedHash.Length == 0) throw new ArgumentException("Invalid password hash.", nameof(storedHash));
            if (storedSalt == null || storedSalt.Length == 0) throw new ArgumentException("Invalid password salt.", nameof(storedSalt));

            using (var hmac = new HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(storedHash);
            }
        }

        private byte[] GenerateSalt()
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                var salt = new byte[16]; // 128 bits
                rng.GetBytes(salt);
                return salt;
            }
        }

        private byte[] HashPasswordWithSalt(string password, byte[] salt)
        {
            if (string.IsNullOrEmpty(password)) throw new ArgumentException("Password cannot be null or empty.", nameof(password));
            if (salt == null || salt.Length == 0) throw new ArgumentException("Invalid salt.", nameof(salt));

            using (var hmac = new HMACSHA512(salt))
            {
                return hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        public async Task<User> UpdateProfileAsync(Guid userId, CompleteProfileRequest request)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException($"User not found with ID: {userId}");
            }

            // Update profile information
            user.Profile = new Profile
            {
                UserId = userId,
                FirstName = request.FirstName,
                LastName = request.LastName,
                CompanyName = request.CompanyName,
                PhoneNumber = request.PhoneNumber,
                Address = request.Address,
                Industry = request.Industry
            };

            await _userRepository.UpdateAsync(user);

            return user;
        }

        public async Task<User> CompleteProfileAsync(Guid userId, CompleteProfileRequest request)
        {
            var user = await GetUserByIdAsync(userId);
            if (user.Profile != null)
            {
                throw new InvalidOperationException("Profile already completed");
            }

            // Create new profile for initial setup
            user.Profile = new Profile
            {
                UserId = userId,
                FirstName = request.FirstName,
                LastName = request.LastName,
                CompanyName = request.CompanyName,
                PhoneNumber = request.PhoneNumber,
                Address = request.Address,
                Industry = request.Industry,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.UpdateAsync(user);
           

            return user;
        }


        public async Task<User> GetUserByIdAsync(Guid userId)
        {
            return await _userRepository.GetByIdAsync(userId);
        }

        public async Task<int> GetUserCountAsync() => await _userRepository.GetCountAsync();

        public async Task<User> UpdateProfileAsync(Guid userId, UserProfileUpdateRequestModel profileUpdate)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new ArgumentException("User not found.");
            }

            // Update profile details
            if (!string.IsNullOrEmpty(profileUpdate.FirstName))
                user.Profile.FirstName = profileUpdate.FirstName;
            if (!string.IsNullOrEmpty(profileUpdate.LastName))
                user.Profile.LastName = profileUpdate.LastName;
            // Update other profile fields as necessary

            await _userRepository.UpdateAsync(user);
            return user;
        }
    }
}
