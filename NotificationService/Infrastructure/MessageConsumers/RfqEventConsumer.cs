using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using NotificationService.Application.Features.Commands;
using NotificationService.Domain.Entities;
using SharedLibrary.Models.Messages.RfqEvents;

namespace NotificationService.Infrastructure.MessageConsumers
{
    public class RfqEventConsumer :
        IConsumer<RfqCreatedMessage>,
        IConsumer<RfqStatusChangedMessage>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<RfqEventConsumer> _logger;

        public RfqEventConsumer(IMediator mediator, ILogger<RfqEventConsumer> logger)
        {
            _mediator = mediator;
            _logger = logger;
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
                        Data = System.Text.Json.JsonSerializer.Serialize(message)
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

        public async Task Consume(ConsumeContext<RfqStatusChangedMessage> context)
        {
            try
            {
                var message = context.Message;
                await _mediator.Send(new CreateNotificationCommand
                {
                    Notification = new Notification
                    {
                        UserId = message.RfqId, 
                        Message = $"RFQ status has been updated to {message.NewStatus}",
                        Type = "RfqStatusChanged",
                        IsRead = false,
                        Timestamp = DateTime.UtcNow,
                        Data = System.Text.Json.JsonSerializer.Serialize(message)
                    }
                });

                _logger.LogInformation("RFQ status update notification created for RFQ {RfqId}", message.RfqId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing RFQ status update notification");
                throw;
            }
        }
    }
}