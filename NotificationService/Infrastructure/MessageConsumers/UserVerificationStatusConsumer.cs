using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using NotificationService.Application.Features.Commands;
using NotificationService.Domain.Entities;
using SharedLibrary.Models.Messages.UserEvents;

namespace NotificationService.Infrastructure.MessageConsumers
{
    public class UserVerificationStatusConsumer : IConsumer<UserVerificationStatusMessage>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UserVerificationStatusConsumer> _logger;

        public UserVerificationStatusConsumer(IMediator mediator, ILogger<UserVerificationStatusConsumer> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<UserVerificationStatusMessage> context)
        {
            try
            {
                var message = context.Message;
                await _mediator.Send(new CreateNotificationCommand
                {
                    Notification = new Notification
                    {
                        UserId = message.UserId,
                        Message = $"Your account verification status has been updated to: {message.Status}",
                        Type = "VerificationStatus",
                        IsRead = false,
                        Timestamp = DateTime.UtcNow,
                        Data = System.Text.Json.JsonSerializer.Serialize(message)
                    }
                });

                _logger.LogInformation("Verification status notification created for user {UserId}", message.UserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing verification status notification");
                throw;
            }
        }
    }
}