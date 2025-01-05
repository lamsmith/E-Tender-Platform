using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using RFQService.Domain.Entities;

namespace RFQService.Infrastructure.Persistence.Configurations
{
    public class RFQConfiguration : IEntityTypeConfiguration<RFQ>
    {
        public void Configure(EntityTypeBuilder<RFQ> builder)
        {
            builder.ToTable("RFQs");

            // Primary Key
            builder.HasKey(r => r.Id);

            // Properties
            builder.Property(r => r.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(r => r.Description)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(r => r.Deadline)
                .IsRequired();

            builder.Property(r => r.Visibility)
                .IsRequired();

            
            builder.Property(r => r.CreatedByUserId)
                .IsRequired();

            // Relationships with RFQDocument
            builder.HasMany(r => r.Documents)
                .WithOne(d => d.RFQ)
                .HasForeignKey(d => d.RFQId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
