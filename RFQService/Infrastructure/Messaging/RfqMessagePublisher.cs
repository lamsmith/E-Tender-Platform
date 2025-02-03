using SharedLibrary.MessageBroker;
using SharedLibrary.Models.Messages;
using SharedLibrary.Constants;

namespace RFQService.Infrastructure.Messaging
{
    public class RfqMessagePublisher
    {
        private readonly IMessagePublisher _messagePublisher;

        public RfqMessagePublisher(IMessagePublisher messagePublisher)
        {
            _messagePublisher = messagePublisher;
        }

        public void PublishRfqCreated(string rfqId, string title, string createdBy, DateTime deadline)
        {
            var message = new RfqCreatedMessage
            {
                RfqId = rfqId,
                Title = title,
                CreatedBy = createdBy,
                CreatedAt = DateTime.UtcNow,
                Deadline = deadline
            };

            _messagePublisher.PublishMessage(MessageQueues.RFQCreated, message);
            _messagePublisher.PublishMessage(MessageQueues.Notifications, message);
        }
    }
}