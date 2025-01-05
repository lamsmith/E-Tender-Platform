using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.Persistence.Configurations
{
    public class ProfileConfiguration : IEntityTypeConfiguration<Profile>
    {
        public void Configure(EntityTypeBuilder<Profile> builder)
        {
            builder.ToTable("Profiles");

            builder.HasKey(p => p.Id);
            builder.Property(p => p.FirstName).IsRequired().HasMaxLength(256);
            builder.Property(p => p.LastName).IsRequired().HasMaxLength(256);
            builder.Property(p => p.PhoneNumber).HasMaxLength(15);
            builder.Property(p => p.Address).HasMaxLength(512);

            builder.HasOne(p => p.User)
                   .WithOne(u => u.Profile)
                   .HasForeignKey<Profile>(p => p.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
