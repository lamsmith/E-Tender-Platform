using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Backoffice_Services.Application.Features.Commands;
using Backoffice_Services.Application.Features.Queries;
using Backoffice_Services.Application.DTO.Requests;
using Backoffice_Services.Infrastructure.Cache;
using Backoffice_Services.Infrastructure.ExternalServices;
using System.Security.Claims;

namespace Backoffice_Services.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class StaffManagementController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICacheService _cacheService;
        private readonly IAuthServiceClient _authServiceClient;
        private readonly ILogger<StaffManagementController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public StaffManagementController(
            IMediator mediator,
            ICacheService cacheService,
            IAuthServiceClient authServiceClient,
            ILogger<StaffManagementController> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _mediator = mediator;
            _cacheService = cacheService;
            _authServiceClient = authServiceClient;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("createstaff")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> CreateStaff([FromBody] CreateStaffRequest request)
        {
            try
            {
                _logger.LogInformation("Received create staff request for email: {Email}", request.Email);

                var loginUser = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var command = new CreateStaffCommand
                {
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    InitiatorUserId = loginUser,
                    PhoneNumber = request.PhoneNumber
                };

                var staffId = await _mediator.Send(command);

                _logger.LogInformation(
                    "Staff creation completed successfully. StaffId: {StaffId}",
                    staffId);

                return CreatedAtAction(
                    nameof(GetStaffById),
                    new { id = staffId },
                    new { message = "Staff created successfully", staffId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating staff member for email: {Email}", request.Email);
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

        [HttpPut("{id}/role")]
        public async Task<IActionResult> UpdateStaffRole(Guid id, [FromBody] UpdateStaffRoleRequest request)
        {
            try
            {
                var command = new UpdateStaffRoleCommand
                {
                    StaffId = id,
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
