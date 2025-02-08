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

                var verificationDtos = new List<UserVerificationDto>();

                foreach (var user in pendingUsers)
                {
                    var userDetails = await _authServiceClient.GetUserDetailsAsync(user.Id);
                    verificationDtos.Add(new UserVerificationDto
                    {
                        UserId = user.Id,
                        Email = user.Email,
                        Role = user.Role,
                        FirstName = userDetails.FirstName,
                        LastName = userDetails.LastName,
                        CompanyName = userDetails.CompanyName,
                        PhoneNumber = userDetails.PhoneNumber,
                        Address = userDetails.Address,
                        Industry = userDetails.Industry,
                        CreatedAt = user.CreatedAt,
                        ProfileCompletedAt = userDetails.ProfileCompletedAt
                    });
                }

                // Apply filters if provided
                if (!string.IsNullOrEmpty(request.Role))
                {
                    verificationDtos = verificationDtos.Where(u => u.Role.Equals(request.Role, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                if (request.FromDate.HasValue)
                {
                    verificationDtos = verificationDtos.Where(u => u.ProfileCompletedAt >= request.FromDate.Value).ToList();
                }

                if (request.ToDate.HasValue)
                {
                    verificationDtos = verificationDtos.Where(u => u.ProfileCompletedAt <= request.ToDate.Value).ToList();
                }

                return verificationDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching pending verifications");
                throw;
            }
        }
    }
}