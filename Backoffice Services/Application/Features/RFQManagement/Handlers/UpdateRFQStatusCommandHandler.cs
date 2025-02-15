using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Backoffice_Services.Infrastructure.ExternalServices;
using Backoffice_Services.Application.Features.RFQManagement.Commands;
using SharedLibrary.Models.Messages.RfqEvents;

namespace Backoffice_Services.Application.Features.RFQManagement.Handlers
{
    public class UpdateRFQStatusCommandHandler : IRequestHandler<UpdateRFQStatusCommand, bool>
    {
        private readonly IRFQServiceClient _rfqServiceClient;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<UpdateRFQStatusCommandHandler> _logger;

        public UpdateRFQStatusCommandHandler(
            IRFQServiceClient rfqServiceClient,
            IPublishEndpoint publishEndpoint,
            ILogger<UpdateRFQStatusCommandHandler> logger)
        {
            _rfqServiceClient = rfqServiceClient;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task<bool> Handle(UpdateRFQStatusCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var rfq = await _rfqServiceClient.GetRFQByIdAsync(request.RFQId);
                if (rfq == null)
                {
                    _logger.LogWarning("RFQ not found: {RFQId}", request.RFQId);
                    return false;
                }

                var result = await _rfqServiceClient.UpdateRFQStatusAsync(request.RFQId, request.Status);

                if (result)
                {
                    await _publishEndpoint.Publish(new RfqStatusUpdatedMessage
                    {
                        RfqId = request.RFQId,
                        NewStatus = request.Status,
                        UpdatedByUserId = rfq.CreatedByUserId,
                        UpdatedAt = DateTime.UtcNow
                    }, cancellationToken);

                    _logger.LogInformation("RFQ status updated. RFQ ID: {RfqId}, New Status: {Status}",
                        request.RFQId, request.Status);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating RFQ status. RFQ ID: {RfqId}", request.RFQId);
                throw;
            }
        }
    }
}
