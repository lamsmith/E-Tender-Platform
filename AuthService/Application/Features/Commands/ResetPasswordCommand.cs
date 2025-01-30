using MediatR;

namespace AuthService.Application.Features.Commands
{
    public class ResetPasswordCommand : IRequest
    {
        public Guid UserId { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}