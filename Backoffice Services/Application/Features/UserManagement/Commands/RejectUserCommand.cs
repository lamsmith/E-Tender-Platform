using MediatR;

namespace Backoffice_Services.Application.Features.UserManagement.Commands
{
    public class RejectUserCommand : IRequest<bool>
    {
        public Guid UserId { get; set; }
        public string RejectionReason { get; set; }
    }
}