using MediatR;

namespace Backoffice_Services.Application.Features.Commands
{
    public class UpdateStaffCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
       
        public bool IsActive { get; set; }
    }
}