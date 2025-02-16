using Microsoft.Extensions.Logging;
using AuthService.Domain.Entities;
using AuthService.Infrastructure.Persistence.Context;
using AuthService.Domain.Enums;
using SharedLibrary.Enums;
using BCrypt.Net;

namespace AuthService.Infrastructure.Persistence.Seeds
{
    public class AuthDbContextSeed
    {
        private readonly ILogger<AuthDbContextSeed> _logger;

        public AuthDbContextSeed(ILogger<AuthDbContextSeed> logger)
        {
            _logger = logger;
        }

        public async Task SeedAsync(AuthDbContext context)
        {
            try
            {
                if (!context.Users.Any())
                {
                    _logger.LogInformation("Starting to seed the Auth database.");

                    var users = new List<User>
                    {
                        new User
                        {
                            Id = Guid.Parse("7d9b7113-a8f8-4035-99a7-a20dd400f6a3"),
                            Email = "admin@example.com",
                            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                            Role = Role.Admin,
                            EmailConfirmed = true,
                            Status = AccountStatus.Verified,
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow
                        },
                        new User
                        {
                            Id = Guid.Parse("2a6b8d68-c1f5-4b47-a8c5-6d6bfa4ef67b"),
                            Email = "vendor@example.com",
                            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Vendor@123"),
                            Role = Role.MSME,
                            EmailConfirmed = true,
                            Status = AccountStatus.Verified,
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow
                        }
                    };

                    await context.Users.AddRangeAsync(users);
                    await context.SaveChangesAsync();

                    _logger.LogInformation("Finished seeding the Auth database.");
                }
                else
                {
                    _logger.LogInformation("Auth database already contains data - skipping seeding.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the Auth database.");
                throw;
            }
        }
    }
}