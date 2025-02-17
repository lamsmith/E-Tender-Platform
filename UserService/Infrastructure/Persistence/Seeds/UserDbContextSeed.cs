using Microsoft.Extensions.Logging;
using UserService.Domain.Entities;
using UserService.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace UserService.Infrastructure.Persistence.Seeds
{
    public class UserDbContextSeed
    {
        private readonly ILogger<UserDbContextSeed> _logger;

        public UserDbContextSeed(ILogger<UserDbContextSeed> logger)
        {
            _logger = logger;
        }

        public async Task SeedAsync(UserDbContext context)
        {
            try
            {
                // Ensure database is created and migrations are applied
                await context.Database.MigrateAsync();

                if (!context.Users.Any())
                {
                    _logger.LogInformation("Starting to seed the database.");

                    var users = new List<User>
                    {
                        new User
                        {
                            Id = Guid.Parse("7d9b7113-a8f8-4035-99a7-a20dd400f6a3"),
                            UserId = Guid.Parse("7d9b7113-a8f8-4035-99a7-a20dd400f6a3"),
                            Email = "admin@example.com",
                            CreatedAt = DateTime.UtcNow,
                            Profile = new Profile
                            {
                                FirstName = "System",
                                LastName = "Admin",
                                CompanyName = null,
                                PhoneNumber = null,
                                RcNumber = null,
                                State = null,
                                City = null,
                                CompanyAddress = null,
                                Industry = null,
                                CreatedAt = DateTime.UtcNow
                            }
                        },
                        new User
                        {
                            Id = Guid.Parse("2a6b8d68-c1f5-4b47-a8c5-6d6bfa4ef67b"),
                            UserId = Guid.Parse("2a6b8d68-c1f5-4b47-a8c5-6d6bfa4ef67b"),
                            Email = "vendor@example.com",
                            CreatedAt = DateTime.UtcNow,
                            Profile = new Profile
                            {
                                FirstName = "Test",
                                LastName = "Vendor",
                                CompanyName = "Test Company Ltd",
                                PhoneNumber = "1234567890",
                                CompanyAddress = "123 Test Street",
                                RcNumber = "RC123456",
                                State = "Test State",
                                City = "Test City",
                                Industry = "Technology",
                                CreatedAt = DateTime.UtcNow
                            }
                        }
                    };

                    await context.Users.AddRangeAsync(users);
                    await context.SaveChangesAsync();

                    _logger.LogInformation("Finished seeding the database.");
                }
                else
                {
                    _logger.LogInformation("Database already contains data - skipping seeding.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }
    }
}
