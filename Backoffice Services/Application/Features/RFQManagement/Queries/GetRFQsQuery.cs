using Backoffice_Services.Application.DTO.RFQManagement.Responses;
using MediatR;
using RFQService.Domain.Paging;

namespace Backoffice_Services.Application.Features.RFQManagement.Queries
{
    public class GetRFQsQuery : IRequest<PaginatedList<RFQResponseModel>>
    {
        public string? Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public PageRequest PageRequest { get; set; }
    }

    
}
