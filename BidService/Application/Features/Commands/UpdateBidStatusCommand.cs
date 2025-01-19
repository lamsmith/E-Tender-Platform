using BidService.Domain.Entities;
using BidService.Domain.Enums;
using MediatR;

namespace BidService.Application.Features.Commands
{
    public class UpdateBidStatusCommand : IRequest<Bid>
    {
        public Guid BidId { get; set; }
        public BidStatus NewStatus { get; set; }
    }
}
