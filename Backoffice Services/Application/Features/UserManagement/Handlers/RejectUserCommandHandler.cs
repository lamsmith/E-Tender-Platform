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
        private readonly IMessagePublisher _messagePublisher;
        private readonly ILogger<RejectUserCommandHandler> _logger;

        public RejectUserCommandHandler(
            IAuthServiceClient authServiceClient,
            IMessagePublisher messagePublisher,
            ILogger<RejectUserCommandHandler> logger)
        {
            _authServiceClient = authServiceClient;
            _messagePublisher = messagePublisher;
            _logger = logger;
        }

        public async Task<bool> Handle(RejectUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _authServiceClient.UpdateUserVerificationStatusAsync(
                    request.UserId,
                    false,
                    request.RejectionReason);

                if (result)
                {
                    var userDetails = await _authServiceClient.GetUserDetailsAsync(request.UserId);

                    var message = new UserVerificationMessage
                    {
                        UserId = request.UserId,
                        Email = userDetails.Email,
                        Status = "Rejected",
                        Notes = request.RejectionReason,
                        VerifiedAt = DateTime.UtcNow
                    };

                    _messagePublisher.PublishMessage(MessageQueues.Notifications, message);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting user: {UserId}", request.UserId);
                throw;
            }
        }
    }
}