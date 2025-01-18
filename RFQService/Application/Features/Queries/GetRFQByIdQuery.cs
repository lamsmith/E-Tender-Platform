using MediatR;
using RFQService.Domain.Entities;

namespace RFQService.Application.Features.Queries
{
    public class GetRFQByIdQuery : IRequest<RFQ>
    {
        public Guid RFQId { get; set; }
    }
}
