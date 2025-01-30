using MediatR;

namespace AuthService.Application.Features.Commands
{
    public class SendStaffWelcomeEmailCommand : IRequest
    {
        public Guid UserId { get; set; }
    }
}