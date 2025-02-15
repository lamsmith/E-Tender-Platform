using MediatR;
using RFQService.Application.Common.Interface.Repositories;
using RFQService.Application.Features.Commands;
using RFQService.Domain.Entities;
using RFQService.Application.Extensions; // Include this for your mapping extensions
using System.Security.Claims;
using MassTransit;
using SharedLibrary.Models.Messages.RfqEvents;

namespace RFQService.Application.Features.Handlers
{
    public class CreateRFQCommandHandler : IRequestHandler<CreateRFQCommand, Guid>
    {
        private readonly IRFQRepository _rfqRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<CreateRFQCommandHandler> _logger;

        public CreateRFQCommandHandler(
            IRFQRepository rfqRepository,
            IHttpContextAccessor httpContextAccessor,
            IPublishEndpoint publishEndpoint,
            ILogger<CreateRFQCommandHandler> logger)
        {
            _rfqRepository = rfqRepository;
            _httpContextAccessor = httpContextAccessor;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task<Guid> Handle(CreateRFQCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    throw new UnauthorizedAccessException("User not authenticated.");
                }

                // Use custom mapping to create RFQ
                var rfq = request.RFQData.ToRFQ();
                rfq.CreatedByUserId = Guid.Parse(userId);

                var createdRFQ = await _rfqRepository.AddAsync(rfq);

                // Handle documents if they are part of the request
                foreach (var docRequest in request.RFQData.Documents)
                {
                    // Use custom mapping for document creation
                    var document = docRequest.ToRFQDocument();
                    document.RFQId = createdRFQ.Id;
                    document.UploadedAt = DateTime.UtcNow;

                    await _rfqRepository.AddDocumentAsync(document);
                }

                // Publish RFQ created event using MassTransit
                await _publishEndpoint.Publish(new RfqCreatedMessage
                {
                    RfqId = createdRFQ.Id,
                    ContractTitle = createdRFQ.ContractTitle,
                    CreatedByUserId = createdRFQ.CreatedByUserId,
                    Visibility = createdRFQ.Visibility.ToString(),
                    CreatedAt = createdRFQ.CreatedAt,
                    Deadline = createdRFQ.Deadline
                }, cancellationToken);

                _logger.LogInformation("RFQ created and event published. RFQ ID: {RfqId}", createdRFQ.Id);

                return createdRFQ.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating RFQ: {Title}", request.RFQData.ContractTitle);
                throw;
            }
        }
    }
}