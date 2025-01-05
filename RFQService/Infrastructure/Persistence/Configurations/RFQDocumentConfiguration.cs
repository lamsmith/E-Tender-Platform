using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using RFQService.Domain.Entities;

namespace RFQService.Infrastructure.Persistence.Configurations
{
    public class RFQDocumentConfiguration : IEntityTypeConfiguration<RFQDocument>
    {
        public void Configure(EntityTypeBuilder<RFQDocument> builder)
        {
            // Table name
            builder.ToTable("RFQDocuments");

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

            builder.Property(d => d.UploadedAt)
                .IsRequired();

            // Relationships
            builder.HasOne(d => d.RFQ)
                .WithMany(r => r.Documents)
                .HasForeignKey(d => d.RFQId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
