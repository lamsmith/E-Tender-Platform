using MediatR;
using Backoffice_Services.Infrastructure.ExternalServices;
using Backoffice_Services.Application.DTO.RFQManagement.Responses;
using Backoffice_Services.Application.Features.RFQManagement.Queries;

namespace Backoffice_Services.Application.Features.RFQManagement.Handlers
{
    public class GetRFQByIdQueryHandler : IRequestHandler<GetRFQByIdQuery, RFQResponseModel>
    {
        private readonly IRFQServiceClient _rfqServiceClient;
        private readonly ILogger<GetRFQByIdQueryHandler> _logger;

        public GetRFQByIdQueryHandler(
            IRFQServiceClient rfqServiceClient,
            ILogger<GetRFQByIdQueryHandler> logger)
        {
            _rfqServiceClient = rfqServiceClient;
            _logger = logger;
        }

        public async Task<RFQResponseModel> Handle(
            GetRFQByIdQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var rfq = await _rfqServiceClient.GetRFQByIdAsync(request.Id);

                if (rfq == null)
                {
                    _logger.LogWarning("RFQ not found: {RFQId}", request.Id);
                    return null;
                }

                _logger.LogInformation("Retrieved RFQ: {RFQId}", request.Id);
                return rfq;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving RFQ: {RFQId}", request.Id);
                throw;
            }
        }
    }
}