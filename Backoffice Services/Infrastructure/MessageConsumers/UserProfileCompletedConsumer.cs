using SharedLibrary.MessageBroker.Implementation;
using SharedLibrary.Models.Messages;
using Microsoft.Extensions.Logging;
using SharedLibrary.Constants;
using Backoffice_Services.Infrastructure.ExternalServices;
using System.Text.Json;

namespace Backoffice_Services.Infrastructure.MessageConsumers
{
    public class UserProfileCompletedConsumer : MessageConsumer
    {
        private readonly IUserProfileServiceClient _userProfileServiceClient;
        private readonly ILogger<UserProfileCompletedConsumer> _logger;

        public UserProfileCompletedConsumer(
            IRabbitMQConnection rabbitMQConnection,
            IUserProfileServiceClient userProfileServiceClient,
            ILogger<UserProfileCompletedConsumer> logger)
            : base(rabbitMQConnection, logger)
        {
            _userProfileServiceClient = userProfileServiceClient;
            _logger = logger;
        }

        protected override async Task ProcessMessage(string messageType, string message)
        {
            try
            {
                var profileCompletedMessage = JsonSerializer.Deserialize<UserProfileCompletedMessage>(message);
                var userDetails = await _userProfileServiceClient.GetUserDetailsAsync(profileCompletedMessage.UserId);

                _logger.LogInformation("Processed profile completion for user: {Email}", userDetails.Email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing user profile completion message");
                throw;
            }
        }
    }
}