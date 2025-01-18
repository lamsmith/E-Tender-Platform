using System.Security.Cryptography;
using System.Text;
using UserService.Application.Common.Interface.Repositories;
using UserService.Application.Common.Interface.Services;
using UserService.Domain.Entities;
using UserService.DTO.Requests;

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

        public async Task ChangePasswordAsync(Guid userId, UserChangePasswordRequestModel changePassword)
        {
            if (changePassword == null) throw new ArgumentNullException(nameof(changePassword));

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new ArgumentException("User not found.", nameof(userId));
            }

            // Convert stored hash and salt from Base64 string to byte array
            var storedHash = Convert.FromBase64String(user.PasswordHash);
            var storedSalt = Convert.FromBase64String(user.PasswordSalt);

            if (!VerifyPassword(changePassword.CurrentPassword, storedHash, storedSalt))
            {
                throw new UnauthorizedAccessException("Current password is incorrect.");
            }

            var newSalt = GenerateSalt();
            var newHash = HashPasswordWithSalt(changePassword.NewPassword, newSalt);

            // Store the new hash and salt as Base64 strings
            user.PasswordHash = Convert.ToBase64String(newHash);
            user.PasswordSalt = Convert.ToBase64String(newSalt);

            await _userRepository.UpdateAsync(user);
        }



        public async Task<int> GetUserBidsSubmittedCountAsync(Guid userId)
        {
            // Assuming there's a method in the repository to fetch this count
            return await _userRepository.GetBidsSubmittedCountAsync(userId);
        }

        public async Task<decimal> GetUserBidSuccessRateAsync(Guid userId)
        {
            // Calculation for bid success rate; assuming there's a method to get this data
            var bidStats = await _userRepository.GetBidSuccessRateAsync(userId);
            if (bidStats.TotalBids == 0) return 0;
            return (decimal)bidStats.SuccessfulBids / bidStats.TotalBids * 100;
        }

        public async Task<User> GetUserByIdAsync(Guid userId)
        {
            return await _userRepository.GetByIdAsync(userId);
        }

     

        public async Task<int> GetUserRfqCreatedCountAsync(Guid userId)
        {
            // Counting RFQs created by a user
            return await _userRepository.GetRfqCreatedCountAsync(userId);
        }

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
