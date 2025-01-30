using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Backoffice_Services.Domain.Entities;

namespace Backoffice_Services.Infrastructure.Persistence.Configurations
{
    public class StaffPermissionConfiguration : IEntityTypeConfiguration<StaffPermission>
    {
        public void Configure(EntityTypeBuilder<StaffPermission> builder)
        {
            builder.HasOne(sp => sp.Staff)
                   .WithMany(s => s.Permissions)
                   .HasForeignKey(sp => sp.StaffId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}