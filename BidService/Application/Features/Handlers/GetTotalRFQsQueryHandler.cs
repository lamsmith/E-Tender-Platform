using BidService.Application.Common;
using BidService.Application.Features.Queries;
using BidService.Domain.Entities;
using BidService.Domain.Paging;
using MediatR;

namespace BidService.Application.Features.Handlers
{
    public class GetTotalBidsQueryHandler : IRequestHandler<GetTotalBidsByUserQuery, PaginatedList<Bid>>
    {
        private readonly IBidRepository _bidRepository;

        public GetTotalBidsQueryHandler(IBidRepository bidRepository)
        {
            _bidRepository = bidRepository;
        }

        public async Task<PaginatedList<Bid>> Handle(GetTotalBidsByUserQuery request, CancellationToken cancellationToken)
        {
            return await _bidRepository.CountBidsByUserAsync(request.UserId, request.PageRequest);
        }
    }
}
