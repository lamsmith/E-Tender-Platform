using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserService.Domain.Entities;
using UserService.Infrastructure.Persistence.Context;

namespace UserService.Infrastructure.Persistence.Seeds
{
    public class UserDbContextSeed
    {
        public static async Task SeedAsync(UserDbContext context, ILogger<UserDbContextSeed> logger)
        {
            try
            {
                if (!await context.Users.AnyAsync())
                {
                    var userId = Guid.NewGuid();
                    var user = new User
                    {
                        Id = Guid.NewGuid(),
                        Email = "admin@etender.com",
                        UserId = userId,
                        Profile = new Profile
                        {
                            Id = Guid.NewGuid(),
                            UserId = userId,
                            FirstName = "Admin",
                            LastName = "User",
                            CompanyName = "E-Tender Platform",
                            PhoneNumber = "+1234567890",
                            Address = "Admin Address",
                            Industry = "Technology"
                        }
                    };

                    await context.Users.AddAsync(user);
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