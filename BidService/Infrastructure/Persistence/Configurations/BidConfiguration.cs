using BidService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using File = BidService.Domain.Entities.File;

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

            builder.Property(b => b.CostOfProduct)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(b => b.CostOfShipping)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(b => b.Discount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(b => b.Status)
                .IsRequired();

            builder.Property(b => b.SubmissionDate)
                .IsRequired();

            // Required Foreign Keys
            builder.Property(b => b.RFQId)
                .IsRequired();

            builder.Property(b => b.UserId)
                .IsRequired();

            // File relationships
            builder.HasOne(b => b.CompanyProfile)
                .WithOne()
                .HasForeignKey<File>("BidId")
                .IsRequired(false);

            builder.HasOne(b => b.ProjectPlan)
                .WithOne()
                .HasForeignKey<File>("BidId")
                .IsRequired(false);

            builder.HasOne(b => b.ProposalFile)
                .WithOne()
                .HasForeignKey<File>("BidId")
                .IsRequired(false);
        }
    }
}