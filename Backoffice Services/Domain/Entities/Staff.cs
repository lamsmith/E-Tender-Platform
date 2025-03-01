﻿using Backoffice_Services.Domain.Common;
namespace Backoffice_Services.Domain.Entities
{
    public class Staff : BaseEntity
    {
        public Guid UserId { get; set; }  
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? LastLoginAt { get; set; }
    }
}
