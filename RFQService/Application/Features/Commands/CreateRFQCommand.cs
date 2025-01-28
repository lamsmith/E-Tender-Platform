using MediatR;
using RFQService.Application.DTO.Requests;

namespace RFQService.Application.Features.Commands
{
    public class CreateRFQCommand : IRequest<Guid>
    {
        public RFQCreationRequestModel RFQData { get; set; }
    }
}
    