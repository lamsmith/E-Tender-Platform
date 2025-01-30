using Backoffice_Services.Domain.Common;
using Backoffice_Services.Domain.Enums;

namespace Backoffice_Services.Domain.Entities
{
    public class StaffPermission : BaseEntity
    {
        public Guid StaffId { get; set; }
        public Staff Staff { get; set; }
        public PermissionType PermissionType { get; set; }
        public bool IsGranted { get; set; }
    }
}
