using MarketplaceService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MarketplaceService.Infrastructure.Persistence.Context
{
    public class MarketplaceDbContext : DbContext
    {
        public MarketplaceDbContext(DbContextOptions<MarketplaceDbContext> options) : base(options)
        {
        }

        public DbSet<MarketplaceListing> MarketplaceListings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new MarketplaceListingConfiguration());
            base.OnModelCreating(modelBuilder);
        }
    }
}
