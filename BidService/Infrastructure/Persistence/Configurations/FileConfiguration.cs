using BidService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using File = BidService.Domain.Entities.File;

namespace BidService.Infrastructure.Persistence.Configurations
{
    public class FileConfiguration : IEntityTypeConfiguration<File>
    {
        public void Configure(EntityTypeBuilder<File> builder)
        {
            // Table name
            builder.ToTable("Files");

            // Primary Key
            builder.HasKey(f => f.Id);

            // Properties
            builder.Property(f => f.FileName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(f => f.FileUrl)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(f => f.FileType)
                .IsRequired()
                .HasMaxLength(50);

            // Relationships
            builder.Property(f => f.BidId)
              .IsRequired();
        }
    }
}