using Backoffice_Services.Application.DTO.RFQManagement.Responses;
using Backoffice_Services.Application.Features.RFQManagement.Commands;
using Backoffice_Services.Application.DTO.RFQManagement.Common;
using Backoffice_Services.Domain.Paging;

namespace Backoffice_Services.Infrastructure.ExternalServices
{
    public interface IRFQServiceClient
    {
        Task<PaginatedList<RFQResponseModel>> GetRFQsAsync(RFQFilterModel filter, PageRequest pageRequest);
        Task<RFQResponseModel> CreateRFQAsync(CreateRFQCommand request);
        Task<RFQResponseModel> GetRFQByIdAsync(Guid rfqId);
        Task<bool> UpdateRFQStatusAsync(Guid rfqId, string status);
    }
}
