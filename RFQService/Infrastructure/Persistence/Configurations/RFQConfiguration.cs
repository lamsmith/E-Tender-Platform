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
            builder.Property(r => r.ContractTitle)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(r => r.ScopeOfSupply)
                .IsRequired()
                .HasMaxLength(10000);

            builder.Property(r => r.PaymentTerms)
               .IsRequired()
               .HasMaxLength(10000);

            builder.Property(r => r.DeliveryTerms)
               .IsRequired()
               .HasMaxLength(10000);

            builder.Property(r => r.OtherInformation)
              .HasMaxLength(10000);

            builder.Property(r => r.Deadline)
                .IsRequired();

            builder.Property(r => r.Visibility)
                .IsRequired()
                .HasConversion<string>();


            builder.Property(r => r.CreatedByUserId)
                .IsRequired();

            // Relationships with RFQDocument
            builder.HasMany(r => r.Documents)
                .WithOne(d => d.RFQ)
                .HasForeignKey(d => d.RFQId)
                .OnDelete(DeleteBehavior.Cascade);


            // Relationship with RFQRecipient
            builder.HasMany(r => r.Recipients)
                .WithOne() // RFQRecipient doesn't have a navigation property back to RFQ
                .HasForeignKey(rr => rr.RFQId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
