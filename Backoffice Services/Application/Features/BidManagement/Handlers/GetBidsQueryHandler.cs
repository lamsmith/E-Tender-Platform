using Backoffice_Services.Application.DTO.BidManagement.Responses;
using Backoffice_Services.Application.Features.BidManagement.Queries;
using Backoffice_Services.Infrastructure.ExternalServices;
using MediatR;
using Microsoft.Extensions.Logging;
using RFQService.Domain.Paging;

namespace Backoffice_Services.Application.Features.BidManagement.Handlers
{
    public class GetBidsQueryHandler : IRequestHandler<GetBidsQuery, PaginatedList<BidResponseModel>>
    {
        private readonly IBidServiceClient _bidServiceClient;
        private readonly ILogger<GetBidsQueryHandler> _logger;

        public GetBidsQueryHandler(IBidServiceClient bidServiceClient, ILogger<GetBidsQueryHandler> logger)
        {
            _bidServiceClient = bidServiceClient;
            _logger = logger;
        }

        public async Task<PaginatedList<BidResponseModel>> Handle(
            GetBidsQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _bidServiceClient.GetBidsAsync(
                    request.RfqId,
                    request.BidderId,
                    request.Status,
                    request.FromDate,
                    request.ToDate,
                    request.PageRequest);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving bids");
                throw;
            }
        }
    }
}