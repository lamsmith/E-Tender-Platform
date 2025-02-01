using MediatR;
using Backoffice_Services.Application.DTO.Responses;

namespace Backoffice_Services.Application.Features.Queries
{
    public class GetAllStaffQuery : IRequest<List<StaffResponse>>
    {
        // Empty query class as we don't need any parameters
    }
}
