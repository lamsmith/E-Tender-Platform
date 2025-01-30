using MediatR;
using Backoffice_Services.Application.DTO.Responses;

namespace Backoffice_Services.Application.Features.Queries
{
    public class GetStaffByIdQuery : IRequest<StaffResponse>
    {
        public Guid Id { get; set; }
    }
}