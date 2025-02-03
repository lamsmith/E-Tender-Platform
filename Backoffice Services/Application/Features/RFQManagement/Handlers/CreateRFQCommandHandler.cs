using MediatR;
using Backoffice_Services.Infrastructure.ExternalServices;
using Backoffice_Services.Application.DTO.RFQManagement.Responses;
using Backoffice_Services.Application.Features.RFQManagement.Commands;
using SharedLibrary.MessageBroker;
using SharedLibrary.Constants;

namespace Backoffice_Services.Application.Features.RFQManagement.Handlers
{
    public class CreateRFQCommandHandler : IRequestHandler<CreateRFQCommand, RFQResponseModel>
    {
        private readonly IRFQServiceClient _rfqServiceClient;
        private readonly IMessagePublisher _messagePublisher;
        private readonly ILogger<CreateRFQCommandHandler> _logger;

        public CreateRFQCommandHandler(
            IRFQServiceClient rfqServiceClient,
            IMessagePublisher messagePublisher,
            ILogger<CreateRFQCommandHandler> logger)
        {
            _rfqServiceClient = rfqServiceClient;
            _messagePublisher = messagePublisher;
            _logger = logger;
        }

        public async Task<RFQResponseModel> Handle(CreateRFQCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var createdRFQ = await _rfqServiceClient.CreateRFQAsync(request);

                // Publish event for notification
                _messagePublisher.PublishMessage(MessageQueues.RFQCreated, new
                {
                    RFQId = createdRFQ.Id,
                    CreatedByUserId = request.CreatedByUserId,
                    ContractTitle = request.ContractTitle,
                    Visibility = request.Visibility,
                    CreatedAt = DateTime.UtcNow
                });

                _logger.LogInformation(
                    "Created RFQ {RFQId} by user {UserId}",
                    createdRFQ.Id,
                    request.CreatedByUserId);

                return createdRFQ;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating RFQ: {@Request}", request);
                throw;
            }
        }
    }
}