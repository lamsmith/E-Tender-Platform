﻿namespace Dashboard_Analytics_System.Domain.Common
{
    public class BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CreatedBy { get; set; } = default!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
