using MediatR;
using Backoffice_Services.Application.Features.UserManagement.Dtos;

namespace Backoffice_Services.Application.Features.UserManagement.Queries
{
    public class GetPendingVerificationsQuery : IRequest<List<UserVerificationDto>>
    {
        public string? Role { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}