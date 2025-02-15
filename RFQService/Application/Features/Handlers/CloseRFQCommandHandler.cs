using MediatR;
using RFQService.Application.Common.Interface.Repositories;
using RFQService.Application.Features.Commands;
using RFQService.Domain.Enums;
using MassTransit;
using SharedLibrary.Models.Messages.RfqEvents;

namespace RFQService.Application.Features.Handlers
{
    public class CloseRFQCommandHandler : IRequestHandler<CloseRFQCommand, bool>
    {
        private readonly IRFQRepository _rfqRepository;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<CloseRFQCommandHandler> _logger;

        public CloseRFQCommandHandler(
            IRFQRepository rfqRepository,
            IPublishEndpoint publishEndpoint,
            ILogger<CloseRFQCommandHandler> logger)
        {
            _rfqRepository = rfqRepository;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task<bool> Handle(CloseRFQCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var rfq = await _rfqRepository.GetByIdAsync(request.RFQId);
                if (rfq == null) return false;

                rfq.Status = Status.Closed;
                await _rfqRepository.UpdateAsync(rfq);

                // Publish RFQ closed event
                await _publishEndpoint.Publish(new RfqStatusChangedMessage
                {
                    RfqId = rfq.Id,
                    NewStatus = Status.Closed.ToString(),
                    ChangedAt = DateTime.UtcNow
                }, cancellationToken);

                _logger.LogInformation("RFQ closed and event published. RFQ ID: {RfqId}", rfq.Id);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error closing RFQ: {RfqId}", request.RFQId);
                throw;
            }
        }
    }
}
