using SharedLibrary.MessageBroker;
using SharedLibrary.Models.Messages;
using SharedLibrary.Constants;

namespace UserService.Infrastructure.Messaging
{
    public class UserMessagePublisher
    {
        private readonly IMessagePublisher _messagePublisher;

        public UserMessagePublisher(IMessagePublisher messagePublisher)
        {
            _messagePublisher = messagePublisher;
        }

        public void PublishUserCreated(Guid userId, string email, string username)
        {
            var message = new UserCreatedMessage
            {
                UserId = userId,
                Email = email,
                CreatedAt = DateTime.UtcNow
            };

            _messagePublisher.PublishMessage(MessageQueues.UserCreated, message);
            _messagePublisher.PublishMessage(MessageQueues.Notifications, message);
        }
    }
}