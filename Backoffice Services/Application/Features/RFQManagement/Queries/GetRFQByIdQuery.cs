using MediatR;
using Backoffice_Services.Application.DTO.RFQManagement.Responses;

namespace Backoffice_Services.Application.Features.RFQManagement.Queries
{
    public class GetRFQByIdQuery : IRequest<RFQResponseModel>
    {
        public Guid Id { get; set; }
    }
}
