using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
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
                .HasMaxLength(10000)
                .IsRequired(false); 

            builder.Property(r => r.CompanyName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(r => r.Deadline)
                .IsRequired();

            builder.Property(r => r.Visibility)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(r => r.Status)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(r => r.CreatedByUserId)
                .IsRequired()
                .HasColumnType("uuid"); // If using PostgreSQL

            // Relationship with RFQRecipient
            builder.HasMany(r => r.Recipients)
                .WithOne(rr => rr.RFQ)
                .HasForeignKey(rr => rr.RFQId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
