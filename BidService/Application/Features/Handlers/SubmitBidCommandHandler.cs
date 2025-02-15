using BidService.Application.Common;
using BidService.Application.Extensions;
using BidService.Application.Features.Commands;
using BidService.Domain.Entities;
using MediatR;
using MassTransit;
using Microsoft.Extensions.Logging;
using SharedLibrary.Models.Messages.BidEvents;

namespace BidService.Application.Features.Handlers
{
    public class SubmitBidCommandHandler : IRequestHandler<SubmitBidCommand, Bid>
    {
        private readonly IBidRepository _bidRepository;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<SubmitBidCommandHandler> _logger;

        public SubmitBidCommandHandler(
            IBidRepository bidRepository,
            IPublishEndpoint publishEndpoint,
            ILogger<SubmitBidCommandHandler> logger)
        {
            _bidRepository = bidRepository;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task<Bid> Handle(SubmitBidCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var bid = request.BidData.ToBid();
                var createdBid = await _bidRepository.AddAsync(bid);

                // Publish bid submitted event
                await _publishEndpoint.Publish(new BidSubmittedMessage
                {
                    BidId = createdBid.Id,
                    UserId = createdBid.UserId,
                    RfqId = createdBid.RFQId,
                    TotalAmount = createdBid.CostOfProduct + createdBid.CostOfShipping - createdBid.Discount,
                    SubmittedAt = DateTime.UtcNow
                }, cancellationToken);

                _logger.LogInformation("Bid submitted and event published. Bid ID: {BidId}", createdBid.Id);

                return createdBid;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting bid for RFQ: {RfqId}", request.BidData.RFQId);
                throw;
            }
        }
    }
}
