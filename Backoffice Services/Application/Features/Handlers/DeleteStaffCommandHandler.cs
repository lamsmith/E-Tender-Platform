using MediatR;
using Backoffice_Services.Application.Common.Interfaces;
using Backoffice_Services.Application.Features.Commands;

namespace Backoffice_Services.Application.Features.Handlers
{
    public class DeleteStaffCommandHandler : IRequestHandler<DeleteStaffCommand, bool>
    {
        private readonly IStaffRepository _staffRepository;
        private readonly ILogger<DeleteStaffCommandHandler> _logger;

        public DeleteStaffCommandHandler(
            IStaffRepository staffRepository,
            ILogger<DeleteStaffCommandHandler> logger)
        {
            _staffRepository = staffRepository;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteStaffCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var staff = await _staffRepository.GetByIdAsync(request.StaffId);
                if (staff == null)
                    return false;

                await _staffRepository.DeleteAsync(request.StaffId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting staff member with ID: {StaffId}", request.StaffId);
                throw;
            }
        }
    }
}