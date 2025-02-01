using MediatR;
using Backoffice_Services.Domain.Enums;

namespace Backoffice_Services.Application.Features.Commands
{
    public class CreateStaffCommand : IRequest<Guid>
    {

        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public StaffRole Role { get; set; }
        public List<PermissionType> Permissions { get; set; }
    }
}