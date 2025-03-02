using AuthService.Domain.Entities;
using Backoffice_Services.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Infrastructure.Persistence.Context
{
    public class AuthDbContext : DbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserPermission> UserPermissions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AuthDbContext).Assembly);
            modelBuilder.ApplyConfiguration(new UserPermissionConfiguration());
        }
    }
}