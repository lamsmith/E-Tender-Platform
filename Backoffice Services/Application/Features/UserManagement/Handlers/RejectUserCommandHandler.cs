using MediatR;
using Backoffice_Services.Infrastructure.ExternalServices;
using SharedLibrary.MessageBroker;
using SharedLibrary.Models.Messages;
using SharedLibrary.Constants;
using Backoffice_Services.Application.Features.UserManagement.Commands;

namespace Backoffice_Services.Application.Features.UserManagement.Handlers
{
    public class RejectUserCommandHandler : IRequestHandler<RejectUserCommand, bool>
    {
        private readonly IAuthServiceClient _authServiceClient;
        private readonly IUserProfileServiceClient _userProfileClient;
        private readonly IMessagePublisher _messagePublisher;
        private readonly ILogger<RejectUserCommandHandler> _logger;

        public RejectUserCommandHandler(
            IAuthServiceClient authServiceClient,
            IUserProfileServiceClient userProfileClient,
            IMessagePublisher messagePublisher,
            ILogger<RejectUserCommandHandler> logger)
        {
            _authServiceClient = authServiceClient;
            _userProfileClient = userProfileClient;
            _messagePublisher = messagePublisher;
            _logger = logger;
        }

        public async Task<bool> Handle(RejectUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var userProfile = await _userProfileClient.GetUserProfileAsync(request.UserId);

                var result = await _authServiceClient.UpdateAccountStatusAsync(
                    request.UserId,
                    SharedLibrary.Enums.AccountStatus.NotVerified,
                    request.RejectionReason);

                if (result)
                {
                    var message = new UserVerificationMessage
                    {
                        UserId = request.UserId,
                        Status = "Rejected",
                        Notes = request.RejectionReason,
                        VerifiedAt = DateTime.UtcNow
                    };

                    _messagePublisher.PublishMessage(MessageQueues.UserVerification, message);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting user {UserId}", request.UserId);
                throw;
            }
        }
    }
}