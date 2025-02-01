using MediatR;

namespace Backoffice_Services.Application.Features.Commands
{
    public class DeleteStaffCommand : IRequest<bool>
    {
        public Guid StaffId { get; set; }
    }
}