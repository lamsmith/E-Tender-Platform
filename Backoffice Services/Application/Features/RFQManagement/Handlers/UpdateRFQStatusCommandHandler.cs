using MediatR;
using Backoffice_Services.Infrastructure.ExternalServices;
using Backoffice_Services.Application.Features.RFQManagement.Commands;
using SharedLibrary.MessageBroker;
using SharedLibrary.Constants;

namespace Backoffice_Services.Application.Features.RFQManagement.Handlers
{
    public class UpdateRFQStatusCommandHandler : IRequestHandler<UpdateRFQStatusCommand, bool>
    {
        private readonly IRFQServiceClient _rfqServiceClient;
        private readonly IMessagePublisher _messagePublisher;
        private readonly ILogger<UpdateRFQStatusCommandHandler> _logger;

        public UpdateRFQStatusCommandHandler(
            IRFQServiceClient rfqServiceClient,
            IMessagePublisher messagePublisher,
            ILogger<UpdateRFQStatusCommandHandler> logger)
        {
            _rfqServiceClient = rfqServiceClient;
            _messagePublisher = messagePublisher;
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
                    // Publish status update event
                    _messagePublisher.PublishMessage(MessageQueues.RFQStatusUpdated, new
                    {
                        RFQId = request.RFQId,
                        NewStatus = request.Status,
                        ContractTitle = rfq.ContractTitle,
                        UpdatedAt = DateTime.UtcNow
                    });

                    _logger.LogInformation(
                        "Updated RFQ {RFQId} status to {Status}",
                        request.RFQId,
                        request.Status);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating RFQ status: {RFQId}, {Status}",
                    request.RFQId, request.Status);
                throw;
            }
        }
    }
}
