using BidService.Application.Common;
using BidService.Application.Extensions;
using BidService.Application.Features.Commands;
using BidService.Domain.Entities;
using MediatR;

namespace BidService.Application.Features.Handlers
{
    public class SubmitBidCommandHandler : IRequestHandler<SubmitBidCommand, Bid>
    {
        private readonly IBidRepository _bidRepository;
  
        public SubmitBidCommandHandler(IBidRepository bidRepository )
        {
            _bidRepository = bidRepository;
           
        }

        public async Task<Bid> Handle(SubmitBidCommand request, CancellationToken cancellationToken)
        {
         
            var bid = request.BidData.ToBid();
            return await _bidRepository.AddAsync(bid);
        }
    }
   
}
