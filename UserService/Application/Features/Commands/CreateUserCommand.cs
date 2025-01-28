using MediatR;
using UserService.Domain.Entities;

namespace UserService.Application.Features.Commands
{
    public class CreateUserCommand : IRequest<bool>
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<CreateProfileCommand> Profile { get; set; }
    }

    public class CreateProfileCommand : IRequest<bool>
    {
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CompanyName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
    }
}