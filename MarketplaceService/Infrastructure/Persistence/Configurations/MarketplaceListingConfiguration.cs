using MarketplaceService.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace MarketplaceService.Infrastructure.Persistence.Configurations
{
    public class MarketplaceListingConfiguration : IEntityTypeConfiguration<MarketplaceListing>
    {
        public void Configure(EntityTypeBuilder<MarketplaceListing> builder)
        {
            // Table name
            builder.ToTable("MarketplaceListings");

            // Primary Key
            builder.HasKey(m => m.Id);

            // Properties
            builder.Property(m => m.RFQId)
                .IsRequired(); 

            builder.Property(m => m.UserId)
                .IsRequired();

            builder.Property(m => m.IsPublic)
                .IsRequired() 
                .HasDefaultValue(true); 

            builder.Property(m => m.ListedAt)
                .IsRequired(); 

            builder.HasIndex(m => m.RFQId); 
            builder.HasIndex(m => m.UserId);
        }
    }
}
