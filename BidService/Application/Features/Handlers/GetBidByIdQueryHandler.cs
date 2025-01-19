using BidService.Application.Common;
using BidService.Application.Features.Queries;
using BidService.Domain.Entities;
using MediatR;

namespace BidService.Application.Features.Handlers
{
    public class GetBidByIdQueryHandler : IRequestHandler<GetBidByIdQuery, Bid>
    {
        private readonly IBidRepository _bidRepository;

        public GetBidByIdQueryHandler(IBidRepository bidRepository)
        {
            _bidRepository = bidRepository;
        }

        public async Task<Bid> Handle(GetBidByIdQuery request, CancellationToken cancellationToken)
        {
            return await _bidRepository.GetByIdAsync(request.BidId);
        }
    }
}
