using SharedLibrary.MessageBroker;
using SharedLibrary.Models.Messages;
using SharedLibrary.Constants;

namespace BidService.Infrastructure.Messaging
{
    public class BidMessagePublisher
    {
        private readonly IMessagePublisher _messagePublisher;

        public BidMessagePublisher(IMessagePublisher messagePublisher)
        {
            _messagePublisher = messagePublisher;
        }

        public void PublishBidPlaced(string bidId, string userId, string rfqId, decimal amount)
        {
            var message = new BidPlacedMessage
            {
                BidId = bidId,
                UserId = userId,
                RfqId = rfqId,
                Amount = amount,
                PlacedAt = DateTime.UtcNow
            };

            _messagePublisher.PublishMessage(MessageQueues.BidPlaced, message);
            _messagePublisher.PublishMessage(MessageQueues.Notifications, message);
        }
    }
}