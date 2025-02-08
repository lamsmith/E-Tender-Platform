using Backoffice_Services.Application.DTO.BidManagement.Responses;
using Backoffice_Services.Application.Features.BidManagement.Queries;
using Backoffice_Services.Infrastructure.ExternalServices;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Backoffice_Services.Application.Features.BidManagement.Handlers
{
    public class GetBidByIdQueryHandler : IRequestHandler<GetBidByIdQuery, BidResponseModel>
    {
        private readonly IBidServiceClient _bidServiceClient;
        private readonly ILogger<GetBidByIdQueryHandler> _logger;

        public GetBidByIdQueryHandler(IBidServiceClient bidServiceClient, ILogger<GetBidByIdQueryHandler> logger)
        {
            _bidServiceClient = bidServiceClient;
            _logger = logger;
        }

        public async Task<BidResponseModel> Handle(GetBidByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                return await _bidServiceClient.GetBidByIdAsync(request.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving bid: {BidId}", request.Id);
                throw;
            }
        }
    }
}