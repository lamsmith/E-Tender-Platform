using Backoffice_Services.Application.DTO.BidManagement.Responses;
using Backoffice_Services.Application.Features.BidManagement.Commands;
using Backoffice_Services.Infrastructure.ExternalServices;
using MediatR;
using Microsoft.Extensions.Logging;
using MassTransit;
using SharedLibrary.Models.Messages.BidEvents;

namespace Backoffice_Services.Application.Features.BidManagement.Handlers
{
    public class SubmitBidCommandHandler : IRequestHandler<SubmitBidCommand, BidResponseModel>
    {
        private readonly IBidServiceClient _bidServiceClient;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<SubmitBidCommandHandler> _logger;

        public SubmitBidCommandHandler(
            IBidServiceClient bidServiceClient,
            IPublishEndpoint publishEndpoint,
            ILogger<SubmitBidCommandHandler> logger)
        {
            _bidServiceClient = bidServiceClient;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task<BidResponseModel> Handle(SubmitBidCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var bid = await _bidServiceClient.SubmitBidAsync(request);

                await _publishEndpoint.Publish(new BidSubmittedMessage
                {
                    BidId = bid.Id,
                    RfqId = request.RfqId,
                    UserId = request.UserId,
                    Proposal = request.Proposal,
                    CostOfProduct = request.CostOfProduct,
                    CostOfShipping = request.CostOfShipping,
                    Discount = request.Discount,
                    CompanyProfile = request.CompanyProfile != null ? new BidFile
                    {
                        Name = request.CompanyProfile.Name,
                        ContentType = request.CompanyProfile.ContentType,
                        FileUrl = request.CompanyProfile.FileUrl
                    } : null,
                    ProjectPlan = request.ProjectPlan != null ? new BidFile
                    {
                        Name = request.ProjectPlan.Name,
                        ContentType = request.ProjectPlan.ContentType,
                        FileUrl = request.ProjectPlan.FileUrl
                    } : null,
                    ProposalFile = request.ProposalFile != null ? new BidFile
                    {
                        Name = request.ProposalFile.Name,
                        ContentType = request.ProposalFile.ContentType,
                        FileUrl = request.ProposalFile.FileUrl
                    } : null,
                    SubmittedAt = DateTime.UtcNow
                }, cancellationToken);

                return bid;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting bid for RFQ: {RfqId}", request.RfqId);
                throw;
            }
        }
    }
}