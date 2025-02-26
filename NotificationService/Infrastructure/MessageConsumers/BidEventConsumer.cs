using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using NotificationService.Application.Features.Commands;
using NotificationService.Domain.Entities;
using SharedLibrary.Models.Messages.BidEvents;

namespace NotificationService.Infrastructure.MessageConsumers
{
    public class BidEventConsumer :
        IConsumer<BidSubmittedMessage>,
        IConsumer<BidStatusUpdatedMessage>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<BidEventConsumer> _logger;

        public BidEventConsumer(IMediator mediator, ILogger<BidEventConsumer> logger)
        {
            _mediator = mediator;
            _logger = logger;
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
                        Message = $"Your bid for RFQ {message.RfqId} has been submitted successfully.",
                        Type = "BidSubmitted",
                        IsRead = false,
                        Timestamp = DateTime.UtcNow,
                        Data = System.Text.Json.JsonSerializer.Serialize(message)
                    }
                });

                _logger.LogInformation("Bid submission notification created for user {UserId}", message.UserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing bid submission notification");
                throw;
            }
        }

        public async Task Consume(ConsumeContext<BidStatusUpdatedMessage> context)
        {
            try
            {
                var message = context.Message;
                await _mediator.Send(new CreateNotificationCommand
                {
                    Notification = new Notification
                    {
                        UserId = message.BidId, 
                        Message = $"Your bid status has been updated to {message.NewStatus}",
                        Type = "BidStatusUpdated",
                        IsRead = false,
                        Timestamp = DateTime.UtcNow,
                        Data = System.Text.Json.JsonSerializer.Serialize(message)
                    }
                });

                _logger.LogInformation("Bid status update notification created for bid {BidId}", message.BidId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing bid status update notification");
                throw;
            }
        }
    }
}