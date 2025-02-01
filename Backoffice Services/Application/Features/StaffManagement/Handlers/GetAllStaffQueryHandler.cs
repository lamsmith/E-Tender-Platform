using MediatR;
using Backoffice_Services.Application.Common.Interfaces;
using Backoffice_Services.Application.Features.Queries;
using Backoffice_Services.Application.DTO.Responses;
using Backoffice_Services.Infrastructure.Cache;

namespace Backoffice_Services.Application.Features.Handlers
{
    public class GetAllStaffQueryHandler : IRequestHandler<GetAllStaffQuery, List<StaffResponse>>
    {
        private readonly IStaffRepository _staffRepository;
        private readonly ICacheService _cacheService;
        private readonly ILogger<GetAllStaffQueryHandler> _logger;

        public GetAllStaffQueryHandler(
            IStaffRepository staffRepository,
            ICacheService cacheService,
            ILogger<GetAllStaffQueryHandler> logger)
        {
            _staffRepository = staffRepository;
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<List<StaffResponse>> Handle(GetAllStaffQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Try to get from cache
                var cachedStaff = await _cacheService.GetAsync<List<StaffResponse>>("staff_all");
                if (cachedStaff != null)
                    return cachedStaff;

                var staffList = await _staffRepository.GetAllAsync();
                var response = staffList.Select(staff => new StaffResponse
                {
                    Id = staff.Id,
                    UserId = staff.UserId,
                    FirstName = staff.FirstName,
                    LastName = staff.LastName,
                    Email = staff.Email,
                    Role = staff.Role,
                    IsActive = staff.IsActive,
                    Permissions = staff.Permissions
                        .Where(p => p.IsGranted)
                        .Select(p => p.PermissionType)
                        .ToList(),
                    CreatedAt = staff.CreatedAt,
                    LastLoginAt = staff.LastLoginAt
                }).ToList();

                // Cache the response
                await _cacheService.SetAsync("staff_all", response, TimeSpan.FromMinutes(5));

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all staff members");
                throw;
            }
        }
    }
}