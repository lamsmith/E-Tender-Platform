using BidService.Application.Common;
using BidService.Application.Features.Queries;
using BidService.Domain.Entities;
using BidService.Domain.Paging;
using MediatR;

namespace BidService.Application.Features.Handlers
{
    public class GetBidsByRFQQueryHandler : IRequestHandler<GetBidsByRFQQuery, PaginatedList<Bid>>
    {
        private readonly IBidRepository _bidRepository;

        public GetBidsByRFQQueryHandler(IBidRepository bidRepository)
        {
            _bidRepository = bidRepository;
        }

        public async Task<PaginatedList<Bid>> Handle(GetBidsByRFQQuery request, CancellationToken cancellationToken)
        {
            return await _bidRepository.GetBidsByRFQAsync(request.RFQId, request.PageRequest);
        }
    }
}
