using BidService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BidService.Infrastructure.Persistence.Configurations
{
    public class BidConfiguration : IEntityTypeConfiguration<Bid>
    {
        public void Configure(EntityTypeBuilder<Bid> builder)
        {
            builder.ToTable("Bids");

            // Primary Key
            builder.HasKey(b => b.Id);

            // Properties
            builder.Property(b => b.Proposal)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(b => b.Amount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(b => b.Status)
                .IsRequired();

            builder.Property(b => b.SubmissionDate)
                .IsRequired();

            // Relationships
            builder.Property(b => b.RFQId)
               .IsRequired(); // No navigation to RFQ since it's in another service

            builder.Property(b => b.UserId)
                .IsRequired();

            // Relationship with BidDocuments
            builder.HasMany(b => b.ProposalFiles)
                .WithOne(d => d.Bid)
                .HasForeignKey(d => d.BidId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
