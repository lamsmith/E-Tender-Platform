using MediatR;
using RFQService.Domain.Entities;
using RFQService.Domain.Paging;

namespace RFQService.Application.Features.Queries
{
    public class GetAllRFQsQuery : IRequest<PaginatedList<RFQ>>
    {
        public PageRequest PageRequest { get; set; }
    }
}
