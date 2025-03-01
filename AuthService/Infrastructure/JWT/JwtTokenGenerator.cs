using AuthService.Application.Common.Interface.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AuthService.Infrastructure.JWT
{
    public class JwtTokenGenerator : IJwtTokenService
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;

        public JwtTokenGenerator(IConfiguration configuration, IUserRepository userRepository )
        {
            _configuration = configuration;
            _userRepository = userRepository;
        }

        public string GenerateToken(Guid userId, string email, string role)
        {
            var jwtKey = _configuration["Jwt:SecretKey"];
            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new InvalidOperationException("JWT Secret Key is not configured");
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var permissions = _userRepository.GetUserPermissionsAsync(userId).Result; 
            var permissionClaims = permissions.Select(p => new Claim("permission", p.Permission.ToString()));

            var claims = new[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                        new Claim(JwtRegisteredClaimNames.Email, email),
                        new Claim(ClaimTypes.Role, role),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                     }.Concat(permissionClaims); 

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpiryMinutes"] ?? "300")),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public DateTime GetExpirationDate()
        {
            return DateTime.UtcNow.AddHours(1);
        }
    }
}