using Backoffice_Services.Domain.Enums;

namespace Backoffice_Services.Application.DTO.Requests
{
    public class UpdateStaffPermissionsRequest
    {
        public List<PermissionType> Permissions { get; set; }
    }
}