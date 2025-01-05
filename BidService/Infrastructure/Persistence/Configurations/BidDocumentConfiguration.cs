using BidService.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace BidService.Infrastructure.Persistence.Configurations
{
    public class BidDocumentConfiguration : IEntityTypeConfiguration<BidDocument>
    {
        public void Configure(EntityTypeBuilder<BidDocument> builder)
        {
            // Table name
            builder.ToTable("BidDocuments");

            // Primary Key
            builder.HasKey(d => d.Id);

            // Properties
            builder.Property(d => d.FileName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(d => d.FileUrl)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(d => d.FileType)
                .HasMaxLength(50);

            // Relationships
            builder.HasOne(d => d.Bid)
                .WithMany(b => b.BidDocuments)
                .HasForeignKey(d => d.BidId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
