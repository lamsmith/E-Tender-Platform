using BidService.Application.Common;
using BidService.Application.Features.Commands;
using BidService.Domain.Entities;
using MediatR;

namespace BidService.Application.Features.Handlers
{
    public class UpdateBidStatusCommandHandler : IRequestHandler<UpdateBidStatusCommand, Bid>
    {
        private readonly IBidRepository _bidRepository;

        public UpdateBidStatusCommandHandler(IBidRepository bidRepository)
        {
            _bidRepository = bidRepository;
        }

        public async Task<Bid> Handle(UpdateBidStatusCommand request, CancellationToken cancellationToken)
        {
            var bid = await _bidRepository.GetByIdAsync(request.BidId);
            if (bid == null)
            {
                throw new ArgumentException("Bid not found");
            }

            bid.Status = request.NewStatus;
            bid.UpdatedAt = DateTime.UtcNow;
            return await _bidRepository.UpdateAsync(bid);
        }
    }
}
