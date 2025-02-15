using BidService.Application.Common;
using BidService.Application.Features.Commands;
using BidService.Domain.Entities;
using MediatR;
using MassTransit;
using Microsoft.Extensions.Logging;
using SharedLibrary.Models.Messages.BidEvents;

namespace BidService.Application.Features.Handlers
{
    public class UpdateBidStatusCommandHandler : IRequestHandler<UpdateBidStatusCommand, Bid>
    {
        private readonly IBidRepository _bidRepository;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<UpdateBidStatusCommandHandler> _logger;

        public UpdateBidStatusCommandHandler(
            IBidRepository bidRepository,
            IPublishEndpoint publishEndpoint,
            ILogger<UpdateBidStatusCommandHandler> logger)
        {
            _bidRepository = bidRepository;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task<Bid> Handle(UpdateBidStatusCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var bid = await _bidRepository.GetByIdAsync(request.BidId);
                if (bid == null)
                {
                    throw new ArgumentException("Bid not found");
                }

                bid.Status = request.NewStatus;
                bid.UpdatedAt = DateTime.UtcNow;
                var updatedBid = await _bidRepository.UpdateAsync(bid);

                // Publish bid status updated event
                await _publishEndpoint.Publish(new BidStatusUpdatedMessage
                {
                    BidId = updatedBid.Id,
                    NewStatus = updatedBid.Status.ToString(),
                    UpdatedAt = DateTime.UtcNow
                }, cancellationToken);

                _logger.LogInformation("Bid status updated and event published. Bid ID: {BidId}, New Status: {Status}",
                    updatedBid.Id, updatedBid.Status);

                return updatedBid;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating bid status. Bid ID: {BidId}, New Status: {Status}",
                    request.BidId, request.NewStatus);
                throw;
            }
        }
    }
}
