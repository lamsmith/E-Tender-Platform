using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.Persistence.Configurations
{
    public class CompanyLogoConfiguration : IEntityTypeConfiguration<CompanyLogo>
    {
        public void Configure(EntityTypeBuilder<CompanyLogo> builder)
        {
            builder.ToTable("CompanyLogos");

            // Primary Key
            builder.HasKey(cl => cl.Id);

            // Properties
            builder.Property(cl => cl.FileName)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(cl => cl.FileUrl)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.Property(cl => cl.FileType)
                   .IsRequired()
                   .HasMaxLength(50);
        }
    }
}