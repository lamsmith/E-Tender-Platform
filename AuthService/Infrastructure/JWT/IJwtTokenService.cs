namespace AuthService.Infrastructure.JWT
{
    public interface IJwtTokenService
    {
        string GenerateToken(Guid userId, string email, string role);
        string GenerateRefreshToken();
        DateTime GetExpirationDate();
    }
}