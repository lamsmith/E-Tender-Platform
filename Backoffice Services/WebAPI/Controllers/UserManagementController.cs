using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Backoffice_Services.Application.Features.UserManagement.Commands;
using Backoffice_Services.Application.Features.UserManagement.Queries;
using Backoffice_Services.Domain.Enums;

namespace Backoffice_Services.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class UserManagementController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UserManagementController> _logger;

        public UserManagementController(
            IMediator mediator,
            ILogger<UserManagementController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("pending-verifications")]
        public async Task<IActionResult> GetPendingVerifications(
            [FromQuery] string? role,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            try
            {
                var query = new GetPendingVerificationsQuery
                {
                    Role = role,
                    FromDate = fromDate,
                    ToDate = toDate
                };

                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pending verifications");
                return StatusCode(500, new { message = "Error getting pending verifications" });
            }
        }

        [HttpPost("{userId}/approve")]
        public async Task<IActionResult> ApproveUser(Guid userId, [FromBody] ApproveUserRequest request)
        {
            try
            {
                var command = new ApproveUserCommand
                {
                    UserId = userId,
                    Notes = request.Notes
                };

                var result = await _mediator.Send(command);
                return Ok(new { success = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving user: {UserId}", userId);
                return StatusCode(500, new { message = "Error approving user" });
            }
        }

        [HttpPost("{userId}/reject")]
        public async Task<IActionResult> RejectUser(Guid userId, [FromBody] RejectUserRequest request)
        {
            try
            {
                var command = new RejectUserCommand
                {
                    UserId = userId,
                    RejectionReason = request.Reason
                };

                var result = await _mediator.Send(command);
                return Ok(new { success = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting user: {UserId}", userId);
                return StatusCode(500, new { message = "Error rejecting user" });
            }
        }

        [HttpGet("{userId}/onboarding-tasks")]
        [ProducesResponseType(typeof(List<OnboardingTaskDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetOnboardingTasks(Guid userId)
        {
            try
            {
                var query = new GetIncompleteOnboardingTasksQuery { UserId = userId };
                var tasks = await _mediator.Send(query);
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting onboarding tasks for user: {UserId}", userId);
                return StatusCode(500, new { message = "Error retrieving onboarding tasks" });
            }
        }

        [HttpPost("{userId}/send-onboarding-reminder")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SendOnboardingReminder(Guid userId)
        {
            try
            {
                // First get incomplete tasks
                var tasksQuery = new GetIncompleteOnboardingTasksQuery { UserId = userId };
                var incompleteTasks = await _mediator.Send(tasksQuery);

                if (!incompleteTasks.Any())
                {
                    return Ok(new { message = "No incomplete tasks found" });
                }

                // Send reminder
                var command = new SendOnboardingReminderCommand
                {
                    UserId = userId,
                    IncompleteTasks = incompleteTasks.Select(t => t.TaskName).ToList()
                };

                var result = await _mediator.Send(command);
                return Ok(new { success = result, message = "Onboarding reminder sent successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending onboarding reminder for user: {UserId}", userId);
                return StatusCode(500, new { message = "Error sending onboarding reminder" });
            }
        }
    }

    public class ApproveUserRequest
    {
        public string? Notes { get; set; }
    }

    public class RejectUserRequest
    {
        public string Reason { get; set; }
    }
}