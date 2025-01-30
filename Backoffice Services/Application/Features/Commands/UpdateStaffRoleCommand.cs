using MediatR;
using Backoffice_Services.Domain.Enums;

namespace Backoffice_Services.Application.Features.Commands
{
    public class UpdateStaffRoleCommand : IRequest<bool>
    {
        public Guid StaffId { get; set; }
        public StaffRole NewRole { get; set; }
    }
}