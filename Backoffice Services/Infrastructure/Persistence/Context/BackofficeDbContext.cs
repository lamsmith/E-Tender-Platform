using Microsoft.EntityFrameworkCore;
using Backoffice_Services.Domain.Entities;
using Backoffice_Services.Infrastructure.Persistence.Configurations;

namespace Backoffice_Services.Infrastructure.Persistence.Context
{
    public class BackofficeDbContext : DbContext
    {
        public BackofficeDbContext(DbContextOptions<BackofficeDbContext> options)
            : base(options)
        {
        }

        public DbSet<Staff> Staff { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply entity configurations
            modelBuilder.ApplyConfiguration(new StaffConfiguration());
           

        }
    }
}