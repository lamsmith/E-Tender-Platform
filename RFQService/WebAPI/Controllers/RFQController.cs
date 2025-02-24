using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RFQService.Application.Features.Commands;
using RFQService.Application.Features.Queries;
using RFQService.Infrastructure.Cache;
using RFQService.Domain.Entities;
using RFQService.Domain.Paging;
using Microsoft.Extensions.Logging;
using RFQService.Application.DTO.Requests;
using RFQService.Infrastructure.Authorization;
using RFQService.Application.Extensions;
using System.Security.Claims;

namespace RFQService.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RFQController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICacheService _cacheService;
        private readonly ILogger<RFQController> _logger;

        public RFQController(
            IMediator mediator,
            ICacheService cacheService,
            ILogger<RFQController> logger)
        {
            _mediator = mediator;
            _cacheService = cacheService;
            _logger = logger;
        }

        [HttpPost("createRFQ")]
        [AuthorizeRoles("Corporate")]
        public async Task<IActionResult> Create([FromBody] RFQCreationRequestModel request)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("User ID not found in token");
                    return Unauthorized(new { message = "User ID not found in token" });
                }

                _logger.LogInformation(
                    "Creating RFQ request. Title: {Title}, User: {UserId}",
                    request.ContractTitle,
                    userId);

                var command = request.ToCommand(Guid.Parse(userId));
                var result = await _mediator.Send(command);

                _logger.LogInformation(
                    "RFQ created successfully. ID: {RfqId}",
                    result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating RFQ. Title: {Title}", request.ContractTitle);
                return StatusCode(500, new { message = "Error creating RFQ" });
            }
        }

        [HttpGet("{rfqId}")]
        [Authorize(Policy = "RequireAuthenticatedUser")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var cacheKey = $"rfq_{id}";
                var cachedRfq = await _cacheService.GetAsync<RFQ>(cacheKey);
                if (cachedRfq != null)
                {
                    return Ok(cachedRfq);
                }

                var query = new GetRFQByIdQuery { RFQId = id };
                var result = await _mediator.Send(query);

                if (result == null)
                    return NotFound(new { message = "RFQ not found" });

                await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(10));
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving RFQ");
                return StatusCode(500, new { message = "Error retrieving RFQ" });
            }
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll([FromQuery] PageRequest pageRequest)
        {
            try
            {
                var query = new GetAllRFQsQuery { PageRequest = pageRequest };
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving RFQs");
                return StatusCode(500, new { message = "Error retrieving RFQs" });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] RFQUpdateRequestModel request)
        {
            try
            {
                var command = request.ToCommand(id);
                var result = await _mediator.Send(command);
                return result ? Ok() : NotFound();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating RFQ");
                return StatusCode(500, new { message = "Error updating RFQ" });
            }
        }

        [HttpPost("{id}/close")]
        public async Task<IActionResult> Close(Guid id)
        {
            try
            {
                var command = new CloseRFQCommand { RFQId = id };
                var result = await _mediator.Send(command);

                if (!result)
                    return NotFound(new { message = "RFQ not found" });

                // Invalidate cache
                await _cacheService.RemoveAsync($"rfq_{id}");
                var userId = User.FindFirst("userId")?.Value;
                if (!string.IsNullOrEmpty(userId))
                {
                    await _cacheService.RemoveAsync($"rfqs_user_{userId}");
                }

                return Ok(new { message = "RFQ closed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error closing RFQ");
                return StatusCode(500, new { message = "Error closing RFQ" });
            }
        }
    }
}