using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MediatR;
using Backoffice_Services.Application.Features.UserManagement.Queries;
using Backoffice_Services.Application.Features.UserManagement.Dtos;
using Backoffice_Services.Infrastructure.ExternalServices;

namespace Backoffice_Services.Application.Features.UserManagement.Handlers
{
    public class GetPendingVerificationsQueryHandler : IRequestHandler<GetPendingVerificationsQuery, List<UserVerificationDto>>
    {
        private readonly IAuthServiceClient _authServiceClient;
        private readonly ILogger<GetPendingVerificationsQueryHandler> _logger;

        public GetPendingVerificationsQueryHandler(
            IAuthServiceClient authServiceClient,
            ILogger<GetPendingVerificationsQueryHandler> logger)
        {
            _authServiceClient = authServiceClient;
            _logger = logger;
        }

        public async Task<List<UserVerificationDto>> Handle(GetPendingVerificationsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var pendingUsers = await _authServiceClient.GetPendingVerificationsAsync();
                return pendingUsers.Select(u => new UserVerificationDto
                {
                    UserId = u.Id,
                    Email = u.Email,
                    Role = u.Role,
                    CreatedAt = u.CreatedAt
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching pending verifications");
                throw;
            }
        }
    }
}