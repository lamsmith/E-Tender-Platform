using BidService.Application.DTO.Requests;
using BidService.Application.DTO.Responses;
using BidService.Domain.Entities;
using BidService.Domain.Enums;
using BidService.Domain.Paging;

namespace BidService.Application.Extensions
{
    public static class MappingExtensions
    {
        // Mapping for paginated lists, similar to your example
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
                BidAmount = bid.Amount,
                BidStatus = bid.Status,
                ProposalFiles = bid.ProposalFiles.Select(d => new ProposalFileResponseModel
                {
                    FileName = d.FileName,
                    FileType = d.FileType,
                    FileUrl = d.FileUrl
                }).ToList()
            };
        }

        // Mapping for BidDocument if needed separately
        public static ProposalFileResponseModel ToBidDocumentResponse(this ProposalFile document)
        {
            return new ProposalFileResponseModel
            {
                FileName = document.FileName,
                FileType = document.FileType,
                FileUrl = document.FileUrl
            };
        }

        public static Bid ToBid(this BidCreationRequestModel request)
        {
            return new Bid
            {
                RFQId = request.RFQId,
                UserId = request.UserId,
                Amount = request.BidAmount, // Changed from BidAmount to Amount to match entity
                Status = BidStatus.Pending, // Changed from BidStatus to Status to match entity
                SubmissionDate = DateTime.UtcNow, // Changed from CreatedAt to SubmissionDate
                ProposalFiles = request.Documents?.Select(doc => new ProposalFile
                {
                    FileName = doc.Name,
                    FileType = doc.ContentType, 
                    FileUrl = doc.FileUrl
                }).ToList() ?? new List<ProposalFile>() 
            };
        }
    }
}
