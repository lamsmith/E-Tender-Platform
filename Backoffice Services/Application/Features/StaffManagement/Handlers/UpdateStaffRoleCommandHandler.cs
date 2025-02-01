using MediatR;
using Backoffice_Services.Application.Common.Interfaces;
using Backoffice_Services.Application.Features.Commands;

namespace Backoffice_Services.Application.Features.Handlers
{
    public class UpdateStaffRoleCommandHandler : IRequestHandler<UpdateStaffRoleCommand, bool>
    {
        private readonly IStaffRepository _staffRepository;
        private readonly ILogger<UpdateStaffRoleCommandHandler> _logger;

        public UpdateStaffRoleCommandHandler(
            IStaffRepository staffRepository,
            ILogger<UpdateStaffRoleCommandHandler> logger)
        {
            _staffRepository = staffRepository;
            _logger = logger;
        }

        public async Task<bool> Handle(UpdateStaffRoleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var staff = await _staffRepository.GetByIdAsync(request.StaffId);
                if (staff == null)
                    return false;

                staff.Role = request.NewRole;
                await _staffRepository.UpdateAsync(staff);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating role for staff member with ID: {StaffId}", request.StaffId);
                throw;
            }
        }
    }
}