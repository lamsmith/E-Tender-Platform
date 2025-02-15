using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using NotificationService.Application.Features.Commands;
using NotificationService.Domain.Entities;
using SharedLibrary.Models.Messages.UserEvents;

namespace NotificationService.Infrastructure.MessageConsumers
{
    public class OnboardingReminderConsumer : IConsumer<OnboardingReminderMessage>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<OnboardingReminderConsumer> _logger;

        public OnboardingReminderConsumer(IMediator mediator, ILogger<OnboardingReminderConsumer> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<OnboardingReminderMessage> context)
        {
            try
            {
                var message = context.Message;
                await _mediator.Send(new CreateNotificationCommand
                {
                    Notification = new Notification
                    {
                        UserId = message.UserId,
                        Message = "Complete your profile to access all features. Click here to continue your onboarding process.",
                        Type = "OnboardingReminder",
                        IsRead = false,
                        Timestamp = DateTime.UtcNow,
                        Data = System.Text.Json.JsonSerializer.Serialize(message)
                    }
                });

                _logger.LogInformation("Onboarding reminder notification created for user {UserId}", message.UserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing onboarding reminder notification");
                throw;
            }
        }
    }
}