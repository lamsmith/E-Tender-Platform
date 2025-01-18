using MediatR;
using RFQService.Application.DTO.Requests;

namespace RFQService.Application.Features.Commands
{
    public class UpdateRFQCommand : IRequest<bool>
    {
        public Guid RFQId { get; set; }
        public RFQUpdateRequestModel UpdateData { get; set; }
    }
}
