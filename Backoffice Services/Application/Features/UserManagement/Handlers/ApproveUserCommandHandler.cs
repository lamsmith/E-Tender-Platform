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
        private readonly IMessagePublisher _messagePublisher;
        private readonly ILogger<ApproveUserCommandHandler> _logger;

        public ApproveUserCommandHandler(
            IAuthServiceClient authServiceClient,
            IMessagePublisher messagePublisher,
            ILogger<ApproveUserCommandHandler> logger)
        {
            _authServiceClient = authServiceClient;
            _messagePublisher = messagePublisher;
            _logger = logger;
        }

        public async Task<bool> Handle(ApproveUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _authServiceClient.UpdateUserVerificationStatusAsync(
                    request.UserId,
                    true,
                    request.Notes);

                if (result)
                {
                    var userDetails = await _authServiceClient.GetUserDetailsAsync(request.UserId);

                    // Publish verification approved message
                    var message = new UserVerificationMessage
                    {
                        UserId = request.UserId,
                        Email = userDetails.Email,
                        Status = "Approved",
                        Notes = request.Notes,
                        VerifiedAt = DateTime.UtcNow
                    };

                    _messagePublisher.PublishMessage(MessageQueues.Notifications, message);
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