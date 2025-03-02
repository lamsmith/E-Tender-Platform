using MediatR;


namespace Backoffice_Services.Application.Features.Commands
{
    public class CreateStaffCommand : IRequest<Guid>
    {

        public string Email { get; set; }
        public Guid InitiatorUserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }

    }
}