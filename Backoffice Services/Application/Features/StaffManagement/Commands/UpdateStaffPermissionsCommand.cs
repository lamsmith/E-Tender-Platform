using MediatR;
using Backoffice_Services.Domain.Enums;

namespace Backoffice_Services.Application.Features.Commands
{
    public class UpdateStaffPermissionsCommand : IRequest<bool>
    {
        public Guid StaffId { get; set; }
        public List<PermissionType> Permissions { get; set; }
    }
}