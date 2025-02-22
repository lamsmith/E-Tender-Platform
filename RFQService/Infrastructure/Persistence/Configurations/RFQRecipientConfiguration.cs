using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using RFQService.Domain.Entities;

namespace RFQService.Infrastructure.Persistence.Configurations
{
    public class RFQRecipientConfiguration : IEntityTypeConfiguration<RFQRecipient>
    {
        public void Configure(EntityTypeBuilder<RFQRecipient> builder)
        {
            builder.HasKey(rr => new { rr.RFQId, rr.UserId });

            builder.HasOne(rr => rr.RFQ)
                   .WithMany(r => r.Recipients)
                   .HasForeignKey(rr => rr.RFQId)
                   .OnDelete(DeleteBehavior.Cascade);


            builder.HasIndex(rr => rr.RFQId);
            builder.HasIndex(rr => rr.UserId);
            builder.HasIndex(rr => rr.Email);



        }
    }
}
