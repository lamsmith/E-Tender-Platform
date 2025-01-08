﻿using UserService.Domain.Common;

namespace UserService.Domain.Entities
{
    public class User : BaseEntity
    {
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public Guid RoleId { get; set; }
        public Role Role { get; set; }
        public Profile Profile { get; set; }
    }
}
