﻿using UserService.Domain.Common;


namespace UserService.Domain.Entities
{
    public class User : BaseEntity
    {
        public string Email { get; set; }
        public Guid UserId { get; set; }
        public virtual Profile? Profile { get; set; }
    }
}
