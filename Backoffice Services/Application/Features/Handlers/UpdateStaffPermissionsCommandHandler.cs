using MediatR;
using Backoffice_Services.Application.Common.Interfaces;
using Backoffice_Services.Application.Features.Commands;
using Backoffice_Services.Infrastructure.Cache;

namespace Backoffice_Services.Application.Features.Handlers
{
    public class UpdateStaffPermissionsCommandHandler : IRequestHandler<UpdateStaffPermissionsCommand, bool>
    {
        private readonly IStaffRepository _staffRepository;
        private readonly ICacheService _cacheService;
        private readonly ILogger<UpdateStaffPermissionsCommandHandler> _logger;

        public UpdateStaffPermissionsCommandHandler(
            IStaffRepository staffRepository,
            ICacheService cacheService,
            ILogger<UpdateStaffPermissionsCommandHandler> logger)
        {
            _staffRepository = staffRepository;
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<bool> Handle(UpdateStaffPermissionsCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var staff = await _staffRepository.GetByIdAsync(request.StaffId);
                if (staff == null)
                    return false;

                await _staffRepository.UpdatePermissionsAsync(request.StaffId, request.Permissions);

                // Invalidate cache
                await _cacheService.RemoveAsync($"staff_{request.StaffId}");
                await _cacheService.RemoveAsync("staff_all");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating permissions for staff member with ID: {StaffId}", request.StaffId);
                throw;
            }
        }
    }
}
