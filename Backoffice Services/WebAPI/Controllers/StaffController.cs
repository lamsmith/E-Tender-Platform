using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Backoffice_Services.Application.Features.Commands;
using Backoffice_Services.Application.Features.Queries;
using Backoffice_Services.Application.DTO.Requests;
using Backoffice_Services.Infrastructure.Cache;
using Backoffice_Services.Infrastructure.ExternalServices;

namespace Backoffice_Services.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class StaffController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICacheService _cacheService;
        private readonly IAuthServiceClient _authServiceClient;
        private readonly ILogger<StaffController> _logger;

        public StaffController(
            IMediator mediator,
            ICacheService cacheService,
            IAuthServiceClient authServiceClient,
            ILogger<StaffController> logger)
        {
            _mediator = mediator;
            _cacheService = cacheService;
            _authServiceClient = authServiceClient;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateStaff([FromBody] CreateStaffRequest request)
        {
            try
            {
                // First create the user in Auth Service
                var authResult = await _authServiceClient.CreateStaffUserAsync(
                    request.Email,
                    request.Role.ToString());

                // Then create the staff in Backoffice Service
                var command = new CreateStaffCommand
                {
                    Email = request.Email,
                    Password = authResult.TempPassword,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Role = request.Role,
                    Permissions = request.Permissions
                };

                var staffId = await _mediator.Send(command);

                // Notify the user with welcome email
                await _authServiceClient.NotifyStaffUserAsync(authResult.UserId);

                return CreatedAtAction(nameof(GetStaffById), new { id = staffId },
                    new { StaffId = staffId, TempPassword = authResult.TempPassword });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating staff member");
                return StatusCode(500, new { message = "Error creating staff member" });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStaffById(Guid id)
        {
            try
            {
                var query = new GetStaffByIdQuery { Id = id };
                var result = await _mediator.Send(query);

                if (result == null)
                    return NotFound();

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving staff member");
                return StatusCode(500, new { message = "Error retrieving staff member" });
            }
        }

        [HttpPut("{id}/permissions")]
        public async Task<IActionResult> UpdateStaffPermissions(Guid id, [FromBody] UpdateStaffPermissionsRequest request)
        {
            try
            {
                var command = new UpdateStaffPermissionsCommand
                {
                    StaffId = id,
                    Permissions = request.Permissions
                };

                var result = await _mediator.Send(command);
                if (!result)
                    return NotFound();

                await _cacheService.RemoveAsync($"staff_{id}");
                return Ok(new { message = "Staff permissions updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating staff permissions");
                return StatusCode(500, new { message = "Error updating staff permissions" });
            }
        }

        [HttpPut("{id}/role")]
        public async Task<IActionResult> UpdateStaffRole(Guid id, [FromBody] UpdateStaffRoleRequest request)
        {
            try
            {
                var command = new UpdateStaffRoleCommand
                {
                    StaffId = id,
                    NewRole = request.NewRole
                };

                var result = await _mediator.Send(command);
                if (!result)
                    return NotFound();

                await _cacheService.RemoveAsync($"staff_{id}");
                return Ok(new { message = "Staff role updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating staff role");
                return StatusCode(500, new { message = "Error updating staff role" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStaff(Guid id)
        {
            try
            {
                var command = new DeleteStaffCommand { StaffId = id };
                var result = await _mediator.Send(command);

                if (!result)
                    return NotFound();

                await _cacheService.RemoveAsync($"staff_{id}");
                await _cacheService.RemoveAsync("staff_all");

                return Ok(new { message = "Staff member deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting staff member");
                return StatusCode(500, new { message = "Error deleting staff member" });
            }
        }
    }
}
