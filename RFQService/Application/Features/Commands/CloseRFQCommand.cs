using MediatR;

namespace RFQService.Application.Features.Commands
{
    public class CloseRFQCommand : IRequest<bool>
    {
        public Guid RFQId { get; set; }
    }
}
