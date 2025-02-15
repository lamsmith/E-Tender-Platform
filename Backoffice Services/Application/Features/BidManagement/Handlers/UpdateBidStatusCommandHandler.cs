using Backoffice_Services.Application.Features.BidManagement.Commands;
using Backoffice_Services.Infrastructure.ExternalServices;
using MediatR;
using Microsoft.Extensions.Logging;
using MassTransit;

namespace Backoffice_Services.Application.Features.BidManagement.Handlers
{
    public class UpdateBidStatusCommandHandler : IRequestHandler<UpdateBidStatusCommand, bool>
    {
        private readonly IBidServiceClient _bidServiceClient;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<UpdateBidStatusCommandHandler> _logger;

        public UpdateBidStatusCommandHandler(
            IBidServiceClient bidServiceClient,
            IPublishEndpoint publishEndpoint,
            ILogger<UpdateBidStatusCommandHandler> logger)
        {
            _bidServiceClient = bidServiceClient;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task<bool> Handle(UpdateBidStatusCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _bidServiceClient.UpdateBidStatusAsync(request.BidId, request.Status, request.Notes);

                if (result)
                {
                    await _publishEndpoint.Publish(new BidStatusUpdatedMessage
                    {
                        Type = "BidStatusUpdated",
                        BidId = request.BidId,
                        NewStatus = request.Status,
                        Notes = request.Notes,
                        Timestamp = DateTime.UtcNow
                    }, cancellationToken);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating bid status. BidId: {BidId}, Status: {Status}",
                    request.BidId, request.Status);
                throw;
            }
        }
    }
}