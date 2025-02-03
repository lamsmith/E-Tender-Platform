using Backoffice_Services.Domain.Enums;

namespace Backoffice_Services.Application.DTO.Responses
{
    public class StaffResponse
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public StaffRole Role { get; set; }
        public bool IsActive { get; set; }
        public List<PermissionType> Permissions { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
    }
}