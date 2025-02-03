using Backoffice_Services.Domain.Enums;

namespace Backoffice_Services.Application.DTO.Requests
{
    public class UpdateStaffRoleRequest
    {
        public StaffRole NewRole { get; set; }
    }
}