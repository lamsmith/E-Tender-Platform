using MediatR;

namespace Backoffice_Services.Application.Features.Commands
{
    public class UpdateStaffPermissionsCommand : IRequest<bool>
    {
        public Guid StaffId { get; set; }
    }
}