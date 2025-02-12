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
        private readonly IUserProfileServiceClient _userProfileClient;
        private readonly IMessagePublisher _messagePublisher;
        private readonly ILogger<SendOnboardingReminderCommandHandler> _logger;

        public SendOnboardingReminderCommandHandler(
            IUserProfileServiceClient userProfileClient,
            IMessagePublisher messagePublisher,
            ILogger<SendOnboardingReminderCommandHandler> logger)
        {
            _userProfileClient = userProfileClient;
            _messagePublisher = messagePublisher;
            _logger = logger;
        }

        public async Task<bool> Handle(SendOnboardingReminderCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var userProfile = await _userProfileClient.GetUserProfileAsync(request.UserId);
                var incompleteTasks = new List<string>();

                // Check for incomplete profile
                if (!userProfile.IsProfileCompleted)
                {
                    incompleteTasks.Add("Complete your profile information");
                }

                // Add other incomplete task checks as needed

                if (incompleteTasks.Any())
                {
                    var message = new OnboardingReminderMessage
                    {
                        UserId = request.UserId,
                        Email = userProfile.Email,
                        IncompleteTasks = incompleteTasks,
                        DaysRegistered = (int)(DateTime.UtcNow - userProfile.CreatedAt).TotalDays,
                        LastLoginAt = userProfile.LastLoginAt ?? DateTime.UtcNow
                    };

                    _messagePublisher.PublishMessage(MessageQueues.OnboardingReminder, message);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending onboarding reminder for user {UserId}", request.UserId);
                throw;
            }
        }
    }
}