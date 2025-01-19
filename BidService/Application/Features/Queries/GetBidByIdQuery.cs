using BidService.Domain.Entities;
using MediatR;

namespace BidService.Application.Features.Queries
{
    public class GetBidByIdQuery : IRequest<Bid>
    {
        public Guid BidId { get; set; }
    }
}
