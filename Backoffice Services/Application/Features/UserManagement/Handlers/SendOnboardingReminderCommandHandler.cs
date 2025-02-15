using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Backoffice_Services.Application.Features.UserManagement.Commands;
using Backoffice_Services.Infrastructure.ExternalServices;
using SharedLibrary.Models.Messages.UserEvents;

namespace Backoffice_Services.Application.Features.UserManagement.Handlers
{
    public class SendOnboardingReminderCommandHandler : IRequestHandler<SendOnboardingReminderCommand, bool>
    {
        private readonly IUserProfileServiceClient _userServiceClient;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<SendOnboardingReminderCommandHandler> _logger;

        public SendOnboardingReminderCommandHandler(
             IUserProfileServiceClient userServiceClient,
            IPublishEndpoint publishEndpoint,
            ILogger<SendOnboardingReminderCommandHandler> logger)
        {
            _userServiceClient = userServiceClient;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task<bool> Handle(SendOnboardingReminderCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var userDetails = await _userServiceClient.GetUserDetailsAsync(request.UserId);
                if (userDetails == null)
                {
                    _logger.LogWarning("User not found for onboarding reminder. User ID: {UserId}", request.UserId);
                    return false;
                }

                await _publishEndpoint.Publish(new OnboardingReminderMessage
                {
                    UserId = request.UserId,
                    UserEmail = userDetails.Email,
                    CreatedAt = DateTime.UtcNow,
                    ReminderCount = request.ReminderCount
                }, cancellationToken);

                _logger.LogInformation("Onboarding reminder sent for user {UserId}", request.UserId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending onboarding reminder for user {UserId}", request.UserId);
                throw;
            }
        }
    }
}