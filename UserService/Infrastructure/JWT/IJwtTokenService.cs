using System.Security.Claims;

namespace UserService.Infrastructure.JWT
{
    public interface IJwtTokenService
    {
        string GenerateToken(IEnumerable<Claim> claims);
        ClaimsPrincipal? ValidateToken(string token);
    }
}
