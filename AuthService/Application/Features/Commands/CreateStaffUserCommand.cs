using MediatR;
using AuthService.Domain.Enums;

namespace AuthService.Application.Features.Commands
{
    public class CreateStaffUserCommand : IRequest<CreateStaffUserResponse>
    {
        public string Email { get; set; }
        public Role Role { get; set; }
    }

    public class CreateStaffUserResponse
    {
        public Guid UserId { get; set; }
        public string TempPassword { get; set; }
    }
}