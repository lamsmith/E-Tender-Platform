using BidService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BidService.Infrastructure.Persistence.Context
{
    public class BidDbContext : DbContext
    {
        public BidDbContext(DbContextOptions<BidDbContext> options)
            : base(options)
        { }

        public DbSet<Bid> Bids { get; set; }
        public DbSet<ProposalFile> BidDocuments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(BidDbContext).Assembly);
        }
    }
}
