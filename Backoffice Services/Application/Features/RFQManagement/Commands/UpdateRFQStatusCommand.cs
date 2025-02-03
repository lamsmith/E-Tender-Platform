using MediatR;

namespace Backoffice_Services.Application.Features.RFQManagement.Commands
{
    public class UpdateRFQStatusCommand : IRequest<bool>
    {
        public Guid RFQId { get; set; }
        public string Status { get; set; }
    }
}
