using MassTransit;
using MediatR;
using NotificationService.Application.Features.Commands;
using NotificationService.Domain.Entities;
using SharedLibrary.Models.Messages;
using SharedLibrary.Models.Messages.BidEvents;
using SharedLibrary.Models.Messages.RfqEvents;
using System.Text.Json;

namespace NotificationService.Infrastructure.MessageConsumers
{
    public class NotificationConsumer :
        IConsumer<UserCreatedMessage>,
        IConsumer<BidSubmittedMessage>,
        IConsumer<RfqCreatedMessage>,
        IConsumer<KycSubmittedMessage>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<NotificationConsumer> _logger;

        public NotificationConsumer(
            IMediator mediator,
            ILogger<NotificationConsumer> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<UserCreatedMessage> context)
        {
            try
            {
                var message = context.Message;
                await _mediator.Send(new CreateNotificationCommand
                {
                    Notification = new Notification
                    {
                        UserId = message.UserId,
                        Message = $"Welcome {message.FirstName}! Your account has been created successfully.",
                        Type = "UserCreated",
                        IsRead = false,
                        Timestamp = DateTime.UtcNow,
                        Data = JsonSerializer.Serialize(message)
                    }
                });

                _logger.LogInformation("User creation notification created for user {UserId}", message.UserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing user creation notification");
                throw;
            }
        }

        public async Task Consume(ConsumeContext<BidSubmittedMessage> context)
        {
            try
            {
                var message = context.Message;
                await _mediator.Send(new CreateNotificationCommand
                {
                    Notification = new Notification
                    {
                        UserId = message.UserId,
                        Message = $"Your bid for RFQ {message.RfqId} has been placed successfully.",
                        Type = "BidPlaced",
                        IsRead = false,
                        Timestamp = DateTime.UtcNow,
                        Data = JsonSerializer.Serialize(message)
                    }
                });

                _logger.LogInformation("Bid placement notification created for user {UserId}", message.UserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing bid placement notification");
                throw;
            }
        }

        public async Task Consume(ConsumeContext<RfqCreatedMessage> context)
        {
            try
            {
                var message = context.Message;
                await _mediator.Send(new CreateNotificationCommand
                {
                    Notification = new Notification
                    {
                        UserId = message.CreatedByUserId,
                        Message = $"Your RFQ '{message.ContractTitle}' has been created successfully.",
                        Type = "RfqCreated",
                        IsRead = false,
                        Timestamp = DateTime.UtcNow,
                        Data = JsonSerializer.Serialize(message)
                    }
                });

                _logger.LogInformation("RFQ creation notification created for user {UserId}", message.CreatedByUserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing RFQ creation notification");
                throw;
            }
        }

        public async Task Consume(ConsumeContext<KycSubmittedMessage> context)
        {
            try
            {
                var message = context.Message;
                await _mediator.Send(new CreateNotificationCommand
                {
                    Notification = new Notification
                    {
                        UserId = message.UserId,
                        Message = "Your KYC documents have been submitted and are pending review.",
                        Type = "KycSubmitted",
                        IsRead = false,
                        Timestamp = DateTime.UtcNow,
                        Data = JsonSerializer.Serialize(message)
                    }
                });

                _logger.LogInformation("KYC submission notification created for user {UserId}", message.UserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing KYC submission notification");
                throw;
            }
        }
    }
}