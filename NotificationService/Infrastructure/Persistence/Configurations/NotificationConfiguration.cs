using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using NotificationService.Domain.Entities;

namespace NotificationService.Infrastructure.Persistence.Configurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            // Table name
            builder.ToTable("Notifications");

            // Primary Key
            builder.HasKey(n => n.Id);

            // Properties
            builder.Property(n => n.Message)
                .IsRequired()
                .HasMaxLength(1000); 
            builder.Property(n => n.Timestamp)
                .IsRequired();

            builder.Property(n => n.IsRead)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(n => n.UserId)
                .IsRequired(); 

          
        }
    }
}
