﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Backoffice_Services.Domain.Entities;

namespace Backoffice_Services.Infrastructure.Persistence.Configurations
{
    public class StaffConfiguration : IEntityTypeConfiguration<Staff>
    {
        public void Configure(EntityTypeBuilder<Staff> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.UserId)
                .IsRequired();

            builder.Property(s => s.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(s => s.LastName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(s => s.Email)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(s => s.Role)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(s => s.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.HasMany(s => s.Permissions)
                .WithOne(p => p.Staff)
                .HasForeignKey(p => p.StaffId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(s => s.UserId)
                .IsUnique();

            builder.HasIndex(s => s.Email)
                .IsUnique();
        }
    }
}