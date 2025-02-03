using Backoffice_Services.Domain.Enums;

namespace Backoffice_Services.Application.DTO.Requests
{
    public class CreateStaffRequest
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public StaffRole Role { get; set; }
        public List<PermissionType> Permissions { get; set; }
    }
}