using BidService.Domain.Entities;
using BidService.Domain.Paging;
using MediatR;

namespace BidService.Application.Features.Queries
{
    public class GetBidsByRFQQuery : IRequest<PaginatedList<Bid>>
    {
        public Guid RFQId { get; set; }
        public PageRequest PageRequest { get; set; }
    }
}
