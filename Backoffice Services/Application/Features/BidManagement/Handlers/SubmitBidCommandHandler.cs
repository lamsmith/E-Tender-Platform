using Backoffice_Services.Application.DTO.BidManagement.Responses;
using Backoffice_Services.Application.Features.BidManagement.Commands;
using Backoffice_Services.Infrastructure.ExternalServices;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedLibrary.MessageBroker;
using SharedLibrary.Constants;

namespace Backoffice_Services.Application.Features.BidManagement.Handlers
{
    public class SubmitBidCommandHandler : IRequestHandler<SubmitBidCommand, BidResponseModel>
    {
        private readonly IBidServiceClient _bidServiceClient;
        private readonly IMessagePublisher _messagePublisher;
        private readonly ILogger<SubmitBidCommandHandler> _logger;

        public SubmitBidCommandHandler(
            IBidServiceClient bidServiceClient,
            IMessagePublisher messagePublisher,
            ILogger<SubmitBidCommandHandler> logger)
        {
            _bidServiceClient = bidServiceClient;
            _messagePublisher = messagePublisher;
            _logger = logger;
        }

        public async Task<BidResponseModel> Handle(SubmitBidCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var bid = await _bidServiceClient.SubmitBidAsync(request);

                // Publish notification for bid submission
                _messagePublisher.PublishMessage(MessageQueues.Notifications, new
                {
                    Type = "BidSubmitted",
                    BidId = bid.Id,
                    RfqId = bid.RfqId,
                    UserId = bid.UserId,
                    Timestamp = DateTime.UtcNow
                });

                return bid;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting bid for RFQ: {RfqId}", request.RfqId);
                throw;
            }
        }
    }
}