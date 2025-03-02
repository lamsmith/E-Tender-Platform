using AuthService.Domain.Common;
using AuthService.Domain.Enums;

namespace AuthService.Domain.Entities
{
    public class UserPermission : BaseEntity
    {
        public Guid UserId { get; set; }
        public User User { get; set; }
        public PermissionType Permission { get; set; }
        public bool IsGranted { get; set; }
    }
}
