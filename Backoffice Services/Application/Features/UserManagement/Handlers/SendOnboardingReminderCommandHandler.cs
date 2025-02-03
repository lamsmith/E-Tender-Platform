using MediatR;
using Backoffice_Services.Infrastructure.ExternalServices;
using SharedLibrary.MessageBroker;
using SharedLibrary.Models.Messages;
using SharedLibrary.Constants;
using Backoffice_Services.Application.Features.UserManagement.Commands;

namespace Backoffice_Services.Application.Features.UserManagement.Handlers
{
    public class SendOnboardingReminderCommandHandler : IRequestHandler<SendOnboardingReminderCommand, bool>
    {
        private readonly IAuthServiceClient _authServiceClient;
        private readonly IMessagePublisher _messagePublisher;
        private readonly ILogger<SendOnboardingReminderCommandHandler> _logger;

        public SendOnboardingReminderCommandHandler(
            IAuthServiceClient authServiceClient,
            IMessagePublisher messagePublisher,
            ILogger<SendOnboardingReminderCommandHandler> logger)
        {
            _authServiceClient = authServiceClient;
            _messagePublisher = messagePublisher;
            _logger = logger;
        }

        public async Task<bool> Handle(SendOnboardingReminderCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var userDetails = await _authServiceClient.GetUserDetailsAsync(request.UserId);

                var daysRegistered = (int)(DateTime.UtcNow - userDetails.CreatedAt).TotalDays;

                var message = new OnboardingReminderMessage
                {
                    UserId = request.UserId,
                    Email = userDetails.Email,
                    IncompleteTasks = request.IncompleteTasks,
                    DaysRegistered = daysRegistered,
                    LastLoginAt = userDetails.LastLoginAt ?? userDetails.CreatedAt
                };

                _messagePublisher.PublishMessage(MessageQueues.OnboardingReminder, message);

                _logger.LogInformation(
                    "Sent onboarding reminder for user {UserId} with {Count} incomplete tasks",
                    request.UserId,
                    request.IncompleteTasks.Count);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending onboarding reminder for user: {UserId}", request.UserId);
                throw;
            }
        }
    }
}