﻿using Microsoft.EntityFrameworkCore;
using RFQService.Domain.Entities;
using RFQService.Infrastructure.Persistence.Configurations;

namespace RFQService.Infrastructure.Persistence.Context
{
    public class RFQDbContext : DbContext
    {
        public RFQDbContext(DbContextOptions<RFQDbContext> options) : base(options)
        {
        }

        public DbSet<RFQ> RFQs { get; set; }
        public DbSet<RFQDocument> RFQDocuments { get; set; }
      

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new RFQConfiguration());
            modelBuilder.ApplyConfiguration(new RFQDocumentConfiguration());

            // Apply configurations for other entities if necessary
            base.OnModelCreating(modelBuilder);
        }
    }
}
