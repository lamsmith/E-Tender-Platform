using MassTransit;
using Microsoft.Extensions.Logging;
using RFQService.Application.Common.Interface.Repositories;
using SharedLibrary.Models.Messages.RfqEvents;

namespace RFQService.Infrastructure.MessageConsumers
{
    public class RfqStatusUpdateConsumer : IConsumer<RfqStatusChangedMessage>
    {
        private readonly IRFQRepository _rfqRepository;
        private readonly ILogger<RfqStatusUpdateConsumer> _logger;

        public RfqStatusUpdateConsumer(
            IRFQRepository rfqRepository,
            ILogger<RfqStatusUpdateConsumer> logger)
        {
            _rfqRepository = rfqRepository;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<RfqStatusChangedMessage> context)
        {
            try
            {
                var message = context.Message;
                _logger.LogInformation("Processing RFQ status update for RFQ {RfqId}", message.RfqId);

                var rfq = await _rfqRepository.GetByIdAsync(message.RfqId);
                if (rfq == null)
                {
                    _logger.LogWarning("RFQ not found: {RfqId}", message.RfqId);
                    return;
                }

                // Update cache to reflect the new status
                await _rfqRepository.RemoveCachedRFQAsync(rfq.Id);

                _logger.LogInformation("RFQ status update processed for RFQ {RfqId}", message.RfqId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing RFQ status update for RFQ {RfqId}", context.Message.RfqId);
                throw;
            }
        }
    }
}