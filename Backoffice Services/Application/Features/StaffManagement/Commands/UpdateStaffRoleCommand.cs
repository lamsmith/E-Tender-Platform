using MediatR;


namespace Backoffice_Services.Application.Features.Commands
{
    public class UpdateStaffRoleCommand : IRequest<bool>
    {
        public Guid StaffId { get; set; }
    }
}