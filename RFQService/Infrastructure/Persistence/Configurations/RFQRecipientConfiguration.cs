using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using RFQService.Domain.Entities;

public class RFQRecipientConfiguration : IEntityTypeConfiguration<RFQRecipient>
{
    public void Configure(EntityTypeBuilder<RFQRecipient> builder)
    {
        builder.HasKey(rr => rr.Id); // Match the PK to Id

        builder.HasOne(rr => rr.RFQ)
               .WithMany(r => r.Recipients)
               .HasForeignKey(rr => rr.RFQId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(rr => rr.RFQId);

        builder.HasIndex(rr => rr.Email);
    }
}