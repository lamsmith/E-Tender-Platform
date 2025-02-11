using AuthService.Domain.Entities;
using AuthService.Domain.Enums;
using AuthService.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AuthService.Infrastructure.Persistence.Seeds
{
    public class AuthDbContextSeed
    {
        public static async Task SeedAsync(AuthDbContext context, ILogger<AuthDbContextSeed> logger)
        {
            try
            {
                if (!await context.Users.AnyAsync())
                {
                    await context.Users.AddAsync(new User
                    {
                        Id = Guid.NewGuid(),
                        Email = "admin@etender.com",
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                        Role = Role.SuperAdmin,
                        IsActive = true,
                        EmailConfirmed = true,
                        Status = SharedLibrary.Enums.AccountStatus.Verified,
                        CreatedAt = DateTime.UtcNow
                    });

                    await context.SaveChangesAsync();
                    logger.LogInformation("Seed data added successfully.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }
    }
}