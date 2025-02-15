using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using NotificationService.Application.Features.Commands;
using NotificationService.Domain.Entities;
using SharedLibrary.Models.Messages.EmailEvents;

namespace NotificationService.Infrastructure.MessageConsumers
{
    public class EmailNotificationConsumer : IConsumer<EmailNotificationMessage>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<EmailNotificationConsumer> _logger;

        public EmailNotificationConsumer(IMediator mediator, ILogger<EmailNotificationConsumer> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<EmailNotificationMessage> context)
        {
            try
            {
                var message = context.Message;
                await _mediator.Send(new CreateNotificationCommand
                {
                    Notification = new Notification
                    {
                        UserId = message.UserId,
                        Message = $"Email sent: {message.Subject}",
                        Type = "EmailNotification",
                        IsRead = false,
                        Timestamp = DateTime.UtcNow,
                        Data = System.Text.Json.JsonSerializer.Serialize(new
                        {
                            message.Subject,
                            message.Status,
                            message.SentAt
                        })
                    }
                });

                _logger.LogInformation("Email notification created for user {UserId}", message.UserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing email notification");
                throw;
            }
        }
    }
}