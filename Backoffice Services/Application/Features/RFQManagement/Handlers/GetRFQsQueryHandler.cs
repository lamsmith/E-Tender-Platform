using Backoffice_Services.Application.DTO.RFQManagement.Responses;
using Backoffice_Services.Application.Features.RFQManagement.Queries;
using Backoffice_Services.Infrastructure.ExternalServices;
using MediatR;
using RFQService.Domain.Paging;

namespace Backoffice_Services.Application.Features.RFQManagement.Handlers
{
    public class GetRFQsQueryHandler : IRequestHandler<GetRFQsQuery, PaginatedList<RFQResponseModel>>
    {
        private readonly IRFQServiceClient _rfqServiceClient;
        private readonly ILogger<GetRFQsQueryHandler> _logger;

        public GetRFQsQueryHandler(IRFQServiceClient rfqServiceClient, ILogger<GetRFQsQueryHandler> logger)
        {
            _rfqServiceClient = rfqServiceClient;
            _logger = logger;
        }

        public async Task<PaginatedList<RFQResponseModel>> Handle(
            GetRFQsQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _rfqServiceClient.GetRFQsAsync(filter, request.PageRequest);

                _logger.LogInformation(
                    "Retrieved {Count} RFQs from total of {Total}",
                    result.Items.Count,
                    result.Total);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving RFQs");
                throw;
            }
        }
    }
}