using BidService.Application.DTO.Requests;
using BidService.Application.DTO.Responses;
using BidService.Domain.Entities;
using BidService.Domain.Enums;
using BidService.Domain.Paging;

namespace BidService.Application.Extensions
{
    public static class MappingExtensions
    {
        // Mapping for paginated lists
        public static PaginatedList<T> ToPaginated<T, TB>(this PaginatedList<TB> list, IEnumerable<T> items)
            where T : notnull where TB : notnull
        {
            return new PaginatedList<T>
            {
                Items = items,
                Page = list.Page,
                PageSize = list.PageSize,
                TotalItems = list.TotalItems
            };
        }

        // Mapping from Bid entity to BidResponseModel
        public static BidResponseModel ToBidResponse(this Bid bid)
        {
            return new BidResponseModel
            {
                Id = bid.Id,
                RFQId = bid.RFQId,
                UserId = bid.UserId,
                CostOfProduct = bid.CostOfProduct,
                CostOfShipping = bid.CostOfShipping,
                Discount = bid.Discount,
                Proposal = bid.Proposal,
                BidStatus = bid.Status,
                CompanyProfile = bid.CompanyProfile?.ToFileResponse(),
                ProjectPlan = bid.ProjectPlan?.ToFileResponse(),
                ProposalFiles = bid.ProposalFile?.ToFileResponse()
            };
        }

        // Mapping from Domain.Entities.File to FileResponse
        public static FileResponse ToFileResponse(this Domain.Entities.File document)
        {
            return new FileResponse
            {
                FileName = document.FileName,
                FileType = document.FileType,
                FileUrl = document.FileUrl
            };
        }

        // **NEW: Mapping from DTO.Requests.File to Domain.Entities.File**
        public static Domain.Entities.File ToFile(this DTO.Requests.File file)
        {
            return new Domain.Entities.File
            {
                FileName = file.FileName,
                FileType = file.FileType,
                FileUrl = file.FileUrl
            };
        }

        // Mapping from BidCreationRequestModel to Bid
        public static Bid ToBid(this BidCreationRequestModel request)
        {
            return new Bid
            {
                RFQId = request.RFQId,
                UserId = request.UserId,
                Proposal = request.Proposal,
                CostOfProduct = request.CostOfProduct,
                CostOfShipping = request.CostOfShipping,
                Discount = request.Discount,
                Status = BidStatus.Pending,
                SubmissionDate = DateTime.UtcNow,
                CompanyProfile = request.CompanyProfile?.ToFile(),
                ProjectPlan = request.ProjectPlan?.ToFile(),
                ProposalFile = request.ProposalFiles?.ToFile()
            };
        }
    }
}
