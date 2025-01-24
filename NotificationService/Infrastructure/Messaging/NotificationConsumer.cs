using Microsoft.Extensions.Hosting;
using SharedLibrary.MessageBroker;
using SharedLibrary.Models.Messages;
using SharedLibrary.Constants;
using NotificationService.Services;

namespace NotificationService.Infrastructure.Messaging
{
    public class NotificationConsumer : BackgroundService
    {
        private readonly RabbitMQConsumer<UserCreatedMessage> _userConsumer;
        private readonly RabbitMQConsumer<BidPlacedMessage> _bidConsumer;
        private readonly RabbitMQConsumer<RfqCreatedMessage> _rfqConsumer;
        private readonly INotificationService _notificationService;

        public NotificationConsumer(
            RabbitMQConnection connection,
            INotificationService notificationService)
        {
            _userConsumer = new RabbitMQConsumer<UserCreatedMessage>(connection, MessageQueues.UserCreated);
            _bidConsumer = new RabbitMQConsumer<BidPlacedMessage>(connection, MessageQueues.BidPlaced);
            _rfqConsumer = new RabbitMQConsumer<RfqCreatedMessage>(connection, MessageQueues.RfqCreated);
            _notificationService = notificationService;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _userConsumer.Consume(HandleUserCreated);
            _bidConsumer.Consume(HandleBidPlaced);
            _rfqConsumer.Consume(HandleRfqCreated);

            return Task.CompletedTask;
        }

        private void HandleUserCreated(UserCreatedMessage message)
        {
            _notificationService.SendWelcomeNotification(message.Email, message.Username);
        }

        private void HandleBidPlaced(BidPlacedMessage message)
        {
            _notificationService.SendBidPlacedNotification(message.UserId, message.RfqId, message.Amount);
        }

        private void HandleRfqCreated(RfqCreatedMessage message)
        {
            _notificationService.SendRfqCreatedNotification(message.CreatedBy, message.Title);
        }
    }
}