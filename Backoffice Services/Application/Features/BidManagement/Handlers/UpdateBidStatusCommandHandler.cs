using Backoffice_Services.Application.Features.BidManagement.Commands;
using Backoffice_Services.Infrastructure.ExternalServices;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedLibrary.MessageBroker;
using SharedLibrary.Constants;

namespace Backoffice_Services.Application.Features.BidManagement.Handlers
{
    public class UpdateBidStatusCommandHandler : IRequestHandler<UpdateBidStatusCommand, bool>
    {
        private readonly IBidServiceClient _bidServiceClient;
        private readonly IMessagePublisher _messagePublisher;
        private readonly ILogger<UpdateBidStatusCommandHandler> _logger;

        public UpdateBidStatusCommandHandler(
            IBidServiceClient bidServiceClient,
            IMessagePublisher messagePublisher,
            ILogger<UpdateBidStatusCommandHandler> logger)
        {
            _bidServiceClient = bidServiceClient;
            _messagePublisher = messagePublisher;
            _logger = logger;
        }

        public async Task<bool> Handle(UpdateBidStatusCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _bidServiceClient.UpdateBidStatusAsync(request.BidId, request.Status, request.Notes);

                if (result)
                {
                    // Publish notification for bid status update
                    _messagePublisher.PublishMessage(MessageQueues.Notifications, new
                    {
                        Type = "BidStatusUpdated",
                        BidId = request.BidId,
                        NewStatus = request.Status,
                        Notes = request.Notes,
                        Timestamp = DateTime.UtcNow
                    });
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