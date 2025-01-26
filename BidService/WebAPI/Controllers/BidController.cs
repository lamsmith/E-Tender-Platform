using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BidService.Application.Features.Commands;
using BidService.Application.Features.Queries;
using BidService.Infrastructure.Cache;
using BidService.Domain.Entities;
using BidService.Domain.Paging;
using BidService.Application.DTO.Requests;
using Microsoft.Extensions.Logging;

namespace BidService.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BidController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICacheService _cacheService;
        private readonly ILogger<BidController> _logger;

        public BidController(
            IMediator mediator,
            ICacheService cacheService,
            ILogger<BidController> logger)
        {
            _mediator = mediator;
            _cacheService = cacheService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> SubmitBid([FromBody] BidCreationRequestModel request)
        {
            try
            {
                var command = new SubmitBidCommand { BidData = request };
                var result = await _mediator.Send(command);

                // Invalidate related caches
                await _cacheService.RemoveAsync($"bids_rfq_{request.RFQId}");
                var userId = User.FindFirst("userId")?.Value;
                if (!string.IsNullOrEmpty(userId))
                {
                    await _cacheService.RemoveAsync($"bids_user_{userId}");
                }

                return CreatedAtAction(nameof(GetBidById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting bid");
                return StatusCode(500, new { message = "Error submitting bid" });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBidById(Guid id)
        {
            try
            {
                var cacheKey = $"bid_{id}";
                var cachedBid = await _cacheService.GetAsync<Bid>(cacheKey);
                if (cachedBid != null)
                {
                    return Ok(cachedBid);
                }

                var query = new GetBidByIdQuery { BidId = id };
                var result = await _mediator.Send(query);

                if (result == null)
                    return NotFound(new { message = "Bid not found" });

                await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(10));
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving bid");
                return StatusCode(500, new { message = "Error retrieving bid" });
            }
        }

        [HttpGet("rfq/{rfqId}")]
        public async Task<IActionResult> GetBidsByRFQ(Guid rfqId, [FromQuery] PageRequest pageRequest)
        {
            try
            {
                var cacheKey = $"bids_rfq_{rfqId}_page_{pageRequest.Page}";
                var cachedBids = await _cacheService.GetAsync<PaginatedList<Bid>>(cacheKey);
                if (cachedBids != null)
                {
                    return Ok(cachedBids);
                }

                var query = new GetBidsByRFQQuery
                {
                    RFQId = rfqId,
                    PageRequest = pageRequest
                };
                var result = await _mediator.Send(query);

                await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5));
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving bids for RFQ");
                return StatusCode(500, new { message = "Error retrieving bids" });
            }
        }

        [HttpGet("my-bids")]
        public async Task<IActionResult> GetMyBids([FromQuery] PageRequest pageRequest)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst("userId")?.Value);
                var cacheKey = $"bids_user_{userId}_page_{pageRequest.Page}";

                var cachedBids = await _cacheService.GetAsync<PaginatedList<Bid>>(cacheKey);
                if (cachedBids != null)
                {
                    return Ok(cachedBids);
                }

                var query = new GetTotalBidsByUserQuery
                {
                    UserId = userId,
                    PageRequest = pageRequest
                };
                var result = await _mediator.Send(query);

                await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5));
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user bids");
                return StatusCode(500, new { message = "Error retrieving bids" });
            }
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")] // Only admins can update bid status
        public async Task<IActionResult> UpdateBidStatus(Guid id, [FromBody] UpdateBidStatusCommand command)
        {
            try
            {
                command.BidId = id;
                var result = await _mediator.Send(command);

                // Invalidate related caches
                await _cacheService.RemoveAsync($"bid_{id}");
                await _cacheService.RemoveAsync($"bids_rfq_{result.RFQId}");
                await _cacheService.RemoveAsync($"bids_user_{result.UserId}");

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating bid status");
                return StatusCode(500, new { message = "Error updating bid status" });
            }
        }
    }
}