using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using UserService.Application.Common.Interface.Repositories;
using UserService.Application.Common.Interface.Services;
using UserService.Application.DTO.Requests;
using UserService.Application.DTO.Responses;
using UserService.Domain.Entities;
using UserService.Domain.Enums;

namespace UserService.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
       
        public AuthService(IUserRepository userRepository,
                           IConfiguration configuration,
                           IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;

        }

        public async Task RegisterAsync(UserRegistrationRequestModel request)
        {
            var existingUser = _userRepository.IsExitByEmail(request.Email);
            if (existingUser)
            {
                throw new ArgumentException("A user with the same email already exists.");
            }


            // Check if the role is Admin
            if (request.Role == Role.Admin)
            {
                throw new ArgumentException("Admin role cannot be assigned during registration.");
            }


            // Create user and hash passwood
            var salt = GenerateSalt();
            var hashedPassword = HashPasswordWithSalt(request.Password, salt);


            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                Profile = new Profile
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                },
                CreatedAt = DateTime.UtcNow,
                PasswordHash = hashedPassword,
                PasswordSalt = salt,
                Role = request.Role
            };



            await _userRepository.AddAsync(user);
            
        }

        public async Task<UserLoginResponseModel> LoginAsync(UserLoginRequestModel request)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);

            if (user == null || !VerifyPassword(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                throw new UnauthorizedAccessException("Invalid username or password.");
            }

            var token = await GenerateTokenAsync(user);

            return new UserLoginResponseModel()
            {
                Firstname = user.Profile.FirstName,
                LastName = user.Profile.LastName,
                Email = user.Email,
                Token = token
            };
        }

        public Task LogoutAsync()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            httpContext?.Response.Cookies.Delete("AuthToken");
            return Task.CompletedTask;
        }

        public Task<string> GenerateTokenAsync(User user)
        {
            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
             new Claim("UserId", user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            var token = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
            return Task.FromResult(token);
        }

        private string GenerateSalt()
        {
            var saltBytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

        private string HashPasswordWithSalt(string password, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                var saltedPassword = password + salt;
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private bool VerifyPassword(string password, string storedHash, string storedSalt)
        {
            var hash = HashPasswordWithSalt(password, storedSalt);
            return hash == storedHash;
        }
    }
}
