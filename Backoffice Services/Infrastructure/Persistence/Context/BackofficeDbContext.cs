using Microsoft.EntityFrameworkCore;
using Backoffice_Services.Domain.Entities;

namespace Backoffice_Services.Infrastructure.Persistence.Context
{
    public class BackofficeDbContext : DbContext
    {
        public BackofficeDbContext(DbContextOptions<BackofficeDbContext> options)
            : base(options)
        {
        }

        public DbSet<Staff> Staff { get; set; }
        public DbSet<StaffPermission> StaffPermissions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(BackofficeDbContext).Assembly);
        }
    }
}