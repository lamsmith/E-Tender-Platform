using MassTransit;
using SharedLibrary.Models.Messages.BidEvents;
using Microsoft.Extensions.Logging;

namespace BidService.Infrastructure.Messaging
{
    public class BidMessagePublisher
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<BidMessagePublisher> _logger;

        public BidMessagePublisher(
            IPublishEndpoint publishEndpoint,
            ILogger<BidMessagePublisher> logger)
        {
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task PublishBidPlacedAsync(Guid bidId, Guid userId, Guid rfqId, decimal CostOfProduct, decimal CostOfShipping, decimal Discount)
        {
            try
            {
                var message = new BidSubmittedMessage
                {
                    BidId = bidId,
                    UserId = userId,
                    RfqId = rfqId,
                    CostOfProduct = CostOfProduct,
                    CostOfShipping = CostOfShipping,
                    Discount = Discount,
                    SubmittedAt = DateTime.UtcNow
                };

                await _publishEndpoint.Publish(message);
                _logger.LogInformation("Published bid placed message for bid {BidId}", bidId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing bid placed message for bid {BidId}", bidId);
                throw;
            }
        }
    }
}