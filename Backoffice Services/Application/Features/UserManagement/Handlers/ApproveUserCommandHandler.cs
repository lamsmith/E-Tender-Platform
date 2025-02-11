using MediatR;
using Backoffice_Services.Infrastructure.ExternalServices;
using SharedLibrary.MessageBroker;
using SharedLibrary.Models.Messages;
using SharedLibrary.Constants;
using Backoffice_Services.Application.Features.UserManagement.Commands;

namespace Backoffice_Services.Application.Features.UserManagement.Handlers
{
    public class ApproveUserCommandHandler : IRequestHandler<ApproveUserCommand, bool>
    {
        private readonly IAuthServiceClient _authServiceClient;
        private readonly IUserProfileServiceClient _userProfileServiceClient;
        private readonly IMessagePublisher _messagePublisher;
        private readonly ILogger<ApproveUserCommandHandler> _logger;

        public ApproveUserCommandHandler(
            IAuthServiceClient authServiceClient,
            IUserProfileServiceClient userProfileServiceClient,
            IMessagePublisher messagePublisher,
            ILogger<ApproveUserCommandHandler> logger)
        {
            _authServiceClient = authServiceClient;
            _userProfileServiceClient = userProfileServiceClient;
            _messagePublisher = messagePublisher;
            _logger = logger;
        }

        public async Task<bool> Handle(ApproveUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var userDetails = await _userProfileServiceClient.GetUserDetailsAsync(request.UserId);
                var result = await _authServiceClient.UpdateUserVerificationStatusAsync(request.UserId, true, request.Notes);

                if (result)
                {
                    // Publish verification status message
                    _messagePublisher.PublishMessage(MessageQueues.UserVerification, new UserVerificationStatusChangedMessage
                    {
                        UserId = request.UserId,
                        Email = userDetails.Email,
                        Status = "Approved",
                        Reason = request.Notes,
                        VerifiedAt = DateTime.UtcNow
                    });

                    _logger.LogInformation("User {UserId} approved successfully", request.UserId);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving user: {UserId}", request.UserId);
                throw;
            }
        }
    }
}