using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NotificationService.Application.Features.Commands;
using NotificationService.Domain.Entities;
using SharedLibrary.Constants;
using SharedLibrary.MessageBroker;
using SharedLibrary.Models.Messages;
using System.Text.Json;

namespace NotificationService.Infrastructure.Messaging
{
    public class NotificationConsumer : BackgroundService
    {
        private readonly RabbitMQConnection _connection;
        private readonly IMediator _mediator;
        private readonly ILogger<NotificationConsumer> _logger;

        public NotificationConsumer(
            RabbitMQConnection connection,
            IMediator mediator,
            ILogger<NotificationConsumer> logger)
        {
            _connection = connection;
            _mediator = mediator;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                // Declare the queue using the shared channel
                _connection.Channel.QueueDeclare(
                    queue: MessageQueues.Notifications,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                var consumer = new RabbitMQConsumer<object>(
                    _connection,
                    MessageQueues.Notifications);

                consumer.Consume(async message =>
                {
                    try
                    {
                        await ProcessMessage(message);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing notification message");
                    }
                });

                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(1000, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in notification consumer");
            }
        }

        private async Task ProcessMessage(object message)
        {
            var jsonMessage = JsonSerializer.Serialize(message);

            if (message is UserCreatedMessage userCreated)
            {
                var command = new CreateNotificationCommand
                {
                    Notification = new Notification
                    {
                        UserId = Guid.Parse(userCreated.UserId),
                        Message = $"Welcome {userCreated.FirstName}! Your account has been created successfully.",
                        Type = "UserCreated",
                        IsRead = false,
                        Timestamp = DateTime.UtcNow,
                        Data = jsonMessage
                    }
                };

                await _mediator.Send(command);
            }
            else if (message is BidPlacedMessage bidPlaced)
            {
                var command = new CreateNotificationCommand
                {
                    Notification = new Notification
                    {
                        UserId = Guid.Parse(bidPlaced.UserId),
                        Message = $"Your bid for RFQ {bidPlaced.RfqId} has been placed successfully.",
                        Type = "BidPlaced",
                        IsRead = false,
                        Timestamp = DateTime.UtcNow,
                        Data = jsonMessage
                    }
                };

                await _mediator.Send(command);
            }
            else if (message is RfqCreatedMessage rfqCreated)
            {
                var command = new CreateNotificationCommand
                {
                    Notification = new Notification
                    {
                        UserId = Guid.Parse(rfqCreated.CreatedBy),
                        Message = $"Your RFQ '{rfqCreated.Title}' has been created successfully.",
                        Type = "RfqCreated",
                        IsRead = false,
                        Timestamp = DateTime.UtcNow,
                        Data = jsonMessage
                    }
                };

                await _mediator.Send(command);
            }
        }
    }
}
