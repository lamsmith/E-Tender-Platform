using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Backoffice_Services.Application.Features.UserManagement.Commands;
using Backoffice_Services.Infrastructure.ExternalServices;
using SharedLibrary.Models.Messages.UserEvents;
using SharedLibrary.Enums;

namespace Backoffice_Services.Application.Features.UserManagement.Handlers
{
    public class RejectUserCommandHandler : IRequestHandler<RejectUserCommand, bool>
    {
        private readonly IAuthServiceClient _authServiceClient;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<RejectUserCommandHandler> _logger;

        public RejectUserCommandHandler(
            IAuthServiceClient authServiceClient,
            IPublishEndpoint publishEndpoint,
            ILogger<RejectUserCommandHandler> logger)
        {
            _authServiceClient = authServiceClient;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task<bool> Handle(RejectUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _authServiceClient.UpdateAccountStatusAsync(
                    request.UserId,
                    AccountStatus.NotVerified,
                    request.RejectionReason);

                if (result)
                {
                    await _publishEndpoint.Publish(new UserVerificationStatusMessage
                    {
                        UserId = request.UserId,
                        Status = "Rejected",
                        UpdatedAt = DateTime.UtcNow,
                        Reason = request.RejectionReason
                    }, cancellationToken);

                    _logger.LogInformation("User rejected successfully. User ID: {UserId}", request.UserId);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting user. User ID: {UserId}", request.UserId);
                throw;
            }
        }
    }
}