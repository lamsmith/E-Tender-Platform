using UserService.Domain.Common;
using UserService.Domain.Enums;

namespace UserService.Domain.Entities
{
    public class User : BaseEntity
    {
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public Profile Profile { get; set; }
        public Role Role { get; set; }
    }
}
