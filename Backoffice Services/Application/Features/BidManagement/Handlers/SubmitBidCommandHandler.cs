using Backoffice_Services.Application.DTO.BidManagement.Responses;
using Backoffice_Services.Application.Features.BidManagement.Commands;
using Backoffice_Services.Infrastructure.ExternalServices;
using MediatR;
using Microsoft.Extensions.Logging;
using MassTransit;

namespace Backoffice_Services.Application.Features.BidManagement.Handlers
{
    public class SubmitBidCommandHandler : IRequestHandler<SubmitBidCommand, BidResponseModel>
    {
        private readonly IBidServiceClient _bidServiceClient;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<SubmitBidCommandHandler> _logger;

        public SubmitBidCommandHandler(
            IBidServiceClient bidServiceClient,
            IPublishEndpoint publishEndpoint,
            ILogger<SubmitBidCommandHandler> logger)
        {
            _bidServiceClient = bidServiceClient;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task<BidResponseModel> Handle(SubmitBidCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var bid = await _bidServiceClient.SubmitBidAsync(request);

                // Publish notification for bid submission
                await _publishEndpoint.Publish(new BidSubmittedMessage
                {
                    Type = "BidSubmitted",
                    BidId = bid.Id,
                    RfqId = bid.RfqId,
                    UserId = bid.UserId,
                    Timestamp = DateTime.UtcNow
                }, cancellationToken);

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