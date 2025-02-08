using SharedLibrary.MessageBroker.Implementation;
using SharedLibrary.Models.Messages;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedLibrary.Constants;
using Backoffice_Services.Application.Features.UserManagement.Queries;
using Backoffice_Services.Infrastructure.ExternalServices;

namespace Backoffice_Services.Infrastructure.MessageConsumers
{
    public class UserProfileCompletedConsumer : MessageConsumer
    {
        private readonly IAuthServiceClient _authServiceClient;
        private readonly ILogger<UserProfileCompletedConsumer> _logger;

        public UserProfileCompletedConsumer(
            IAuthServiceClient authServiceClient,
            ILogger<UserProfileCompletedConsumer> logger) : base(logger)
        {
            _authServiceClient = authServiceClient;
        }

        public override async Task ConsumeAsync(string messageType, string message)
        {
            try
            {
                var profileCompletedMessage = System.Text.Json.JsonSerializer.Deserialize<UserProfileCompletedMessage>(message);
                var userDetails = await _authServiceClient.GetUserDetailsAsync(profileCompletedMessage.UserId);

                _logger.LogInformation(
                    "User {UserId} profile completed, pending verification. Email: {Email}, Role: {Role}",
                    profileCompletedMessage.UserId,
                    userDetails.Email,
                    userDetails.Role);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing user profile completion message");
                throw;
            }
        }
    }
}