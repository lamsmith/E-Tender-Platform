using BidService.Domain.Entities;
using BidService.Domain.Paging;
using MediatR;

namespace BidService.Application.Features.Queries
{
    public class GetTotalBidsByUserQuery : IRequest<PaginatedList<Bid>>
    {
        public Guid UserId { get; set; }
        public PageRequest PageRequest { get; set; }
    }
}
