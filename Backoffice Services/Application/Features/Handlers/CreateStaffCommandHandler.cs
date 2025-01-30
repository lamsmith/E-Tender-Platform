using MediatR;
using Backoffice_Services.Application.Common.Interfaces;
using Backoffice_Services.Application.Features.Commands;
using Backoffice_Services.Domain.Entities;
using Backoffice_Services.Infrastructure.Cache;
using Backoffice_Services.Infrastructure.ExternalServices;

namespace Backoffice_Services.Application.Features.Handlers
{
    public class CreateStaffCommandHandler : IRequestHandler<CreateStaffCommand, Guid>
    {
        private readonly IStaffRepository _staffRepository;
        private readonly IAuthServiceClient _authServiceClient;
        private readonly ICacheService _cacheService;
        private readonly ILogger<CreateStaffCommandHandler> _logger;

        public CreateStaffCommandHandler(
            IStaffRepository staffRepository,
            IAuthServiceClient authServiceClient,
            ICacheService cacheService,
            ILogger<CreateStaffCommandHandler> logger)
        {
            _staffRepository = staffRepository;
            _authServiceClient = authServiceClient;
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<Guid> Handle(CreateStaffCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Create user in Auth Service
                var userId = await _authServiceClient.CreateStaffUserAsync(
                    request.Email,
                    request.Password,
                    request.Role.ToString());

                // Create staff in Backoffice Service
                var staff = new Staff
                {
                    UserId = userId,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    Role = request.Role,
                    IsActive = true,
                    Permissions = request.Permissions.Select(p => new StaffPermission
                    {
                        PermissionType = p,
                        IsGranted = true
                    }).ToList()
                };

                var result = await _staffRepository.CreateAsync(staff);

                // Invalidate cache
                await _cacheService.RemoveAsync("staff_all");

                return result.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating staff member");
                throw;
            }
        }
    }
}

