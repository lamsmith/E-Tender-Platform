using RFQService.Application.DTO.Requests;
using RFQService.Application.DTO.Responses;
using RFQService.Domain.Entities;
using RFQService.Domain.Paging;

namespace RFQService.Application.Extensions
{
    public static class MappingExtensions
    {
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

        // Mapping from RFQCreationRequestModel to RFQ
        public static RFQ ToRFQ(this RFQCreationRequestModel request)
        {
            return new RFQ
            {
                ContractTitle = request.ContractTitle,
                ScopeOfSupply = request.ScopeOfSupply,
                PaymentTerms = request.PaymentTerms,
                DeliveryTerms = request.DeliveryTerms,
                OtherInformation = request.OtherInformation,
                Deadline = request.Deadline,
                Visibility = request.Visibility,
                CreatedByUserId = request.CreatedByUserId,
                CreatedAt = DateTime.UtcNow,
                Documents = request.Documents?.Select(doc => new RFQDocument
                {
                    FileName = doc.Name,
                    FileType = doc.ContentType,
                    FileUrl = doc.FileUrl
                }).ToList() ?? new List<RFQDocument>()
            };
        }

        // from RFQ to RFQResponseModel
        public static RFQResponseModel ToRFQResponse(this RFQ rfq)
        {
            return new RFQResponseModel
            {
                Id = rfq.Id,
                ContractTitle = rfq.ContractTitle,
                ScopeOfSupply = rfq.ScopeOfSupply,
                PaymentTerms = rfq.PaymentTerms,
                DeliveryTerms = rfq.DeliveryTerms,
                OtherInformation = rfq.OtherInformation,
                Deadline = rfq.Deadline,
                Visibility = rfq.Visibility,
                CreatedByUserId = rfq.CreatedByUserId,
                CreatedAt = rfq.CreatedAt,
                Documents = rfq.Documents.Select(d => new RFQDocumentResponseModel
                {
                    FileName = d.FileName,
                    FileType = d.FileType,
                    FileUrl = d.FileUrl
                }).ToList()
            };
        }

        public static RFQDocumentResponseModel ToRFQDocumentResponse(this RFQDocument document)
        {
            return new RFQDocumentResponseModel
            {
                FileName = document.FileName,
                FileType = document.FileType,
                FileUrl = document.FileUrl
            };
        }

        public static RFQDocument ToRFQDocument(this DocumentUploadRequest document)
        {
            return new RFQDocument
            {
                FileName = document.Name,
                FileType = document.ContentType,
                FileUrl = document.FileUrl
            };
        }



    }
}
