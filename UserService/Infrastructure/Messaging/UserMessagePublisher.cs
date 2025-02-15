using MassTransit;
using SharedLibrary.Models.Messages;

namespace UserService.Infrastructure.Messaging
{
    public class UserMessagePublisher
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public UserMessagePublisher(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task PublishUserCreatedAsync(Guid userId, string email, string username)
        {
            var message = new UserCreatedMessage
            {
                UserId = userId,
                Email = email,
                CreatedAt = DateTime.UtcNow
            };

            await _publishEndpoint.Publish(message);
        }
    }
}