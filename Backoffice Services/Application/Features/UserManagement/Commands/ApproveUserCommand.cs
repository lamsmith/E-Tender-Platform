using MediatR;

namespace Backoffice_Services.Application.Features.UserManagement.Commands
{
    public class ApproveUserCommand : IRequest<bool>
    {
        public Guid UserId { get; set; }
        public string? Notes { get; set; }
    }
}