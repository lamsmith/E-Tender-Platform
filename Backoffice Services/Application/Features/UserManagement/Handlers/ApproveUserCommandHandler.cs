using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Backoffice_Services.Application.Features.UserManagement.Commands;
using Backoffice_Services.Infrastructure.ExternalServices;
using SharedLibrary.Models.Messages.UserEvents;
using SharedLibrary.Enums;

namespace Backoffice_Services.Application.Features.UserManagement.Handlers
{
    public class ApproveUserCommandHandler : IRequestHandler<ApproveUserCommand, bool>
    {
        private readonly IAuthServiceClient _authServiceClient;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<ApproveUserCommandHandler> _logger;

        public ApproveUserCommandHandler(
            IAuthServiceClient authServiceClient,
            IPublishEndpoint publishEndpoint,
            ILogger<ApproveUserCommandHandler> logger)
        {
            _authServiceClient = authServiceClient;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task<bool> Handle(ApproveUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _authServiceClient.UpdateAccountStatusAsync(
                    request.UserId,
                    AccountStatus.Verified,
                    request.Notes);

                if (result)
                {
                    await _publishEndpoint.Publish(new UserVerificationStatusMessage
                    {
                        UserId = request.UserId,
                        Status = "Approved",
                        UpdatedAt = DateTime.UtcNow,
                        Reason = request.Notes
                    }, cancellationToken);

                    _logger.LogInformation("User approved successfully. User ID: {UserId}", request.UserId);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving user. User ID: {UserId}", request.UserId);
                throw;
            }
        }
    }
}