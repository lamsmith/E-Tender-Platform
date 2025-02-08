using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.Persistence.Configurations
{
    public class ProfileConfiguration : IEntityTypeConfiguration<Profile>
    {
        public void Configure(EntityTypeBuilder<Profile> builder)
        {
            builder.ToTable("Profiles");

            // Primary Key
            builder.HasKey(p => p.Id);

            // Properties
            builder.Property(p => p.FirstName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(p => p.LastName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(p => p.CompanyName)
                   .HasMaxLength(200);

            builder.Property(p => p.PhoneNumber)
                   .HasMaxLength(20);

            builder.Property(p => p.Address)
                   .HasMaxLength(500);

            // One-to-One Relationship with CompanyLogo
            builder.HasOne(p => p.CompanyLogo)
                   .WithMany()
                   .HasForeignKey(p => p.CompanyLogoId)
                   .OnDelete(DeleteBehavior.SetNull); 
        }
    }
}