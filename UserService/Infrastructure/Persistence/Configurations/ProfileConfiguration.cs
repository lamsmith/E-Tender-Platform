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

                     builder.HasKey(p => p.Id);

                     // Required fields
                     builder.Property(p => p.FirstName)
                            .IsRequired()
                            .HasMaxLength(100);

                     builder.Property(p => p.LastName)
                            .IsRequired()
                            .HasMaxLength(100);

                     // Optional fields
                     builder.Property(p => p.CompanyName)
                            .HasMaxLength(200)
                            .IsRequired(false);

                     builder.Property(p => p.PhoneNumber)
                            .HasMaxLength(20)
                            .IsRequired(false);

                     builder.Property(p => p.RcNumber)
                            .HasMaxLength(50)
                            .IsRequired(false);

                     builder.Property(p => p.State)
                            .HasMaxLength(100)
                            .IsRequired(false);

                     builder.Property(p => p.City)
                            .HasMaxLength(100)
                            .IsRequired(false);

                     builder.Property(p => p.CompanyAddress)
                            .HasMaxLength(500)
                            .IsRequired(false);

                     builder.Property(p => p.Industry)
                            .HasMaxLength(100)
                            .IsRequired(false);

                     // Configure one-to-one relationship with CompanyLogo
                     builder.HasOne(p => p.CompanyLogo)
                            .WithMany()
                            .HasForeignKey(p => p.CompanyLogoId)
                            .OnDelete(DeleteBehavior.SetNull);
              }
       }
}
