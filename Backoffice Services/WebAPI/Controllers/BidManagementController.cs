using Backoffice_Services.Application.DTO.BidManagement.Common;
using Backoffice_Services.Application.DTO.BidManagement.Requests;
using Backoffice_Services.Application.Features.BidManagement.Commands;
using Backoffice_Services.Application.Features.BidManagement.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RFQService.Domain.Paging;

namespace Backoffice_Services.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BidManagementController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<BidManagementController> _logger;

        public BidManagementController(
            IMediator mediator,
            ILogger<BidManagementController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Policy = "RequirePermission")]
        public async Task<ActionResult> GetBids(
            [FromQuery] Guid? rfqId,
            [FromQuery] Guid? bidderId,
            [FromQuery] BidStatus? status,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var query = new GetBidsQuery
                {
                    RfqId = rfqId,
                    BidderId = bidderId,
                    Status = status,
                    FromDate = fromDate,
                    ToDate = toDate,
                    PageRequest = new PageRequest { Page = page, PageSize = pageSize }
                };

                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving bids");
                return StatusCode(500, new { message = "Error retrieving bids" });
            }
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "RequirePermission")]
        public async Task<ActionResult> GetBidById(Guid id)
        {
            try
            {
                var bid = await _mediator.Send(new GetBidByIdQuery { Id = id });
                if (bid == null)
                    return NotFound();

                return Ok(bid);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving bid with ID: {BidId}", id);
                return StatusCode(500, new { message = "Error retrieving bid" });
            }
        }

        [HttpPost("submit")]
        [Authorize(Policy = "RequirePermission")]
        public async Task<ActionResult> SubmitBid([FromBody] SubmitBidRequest request)
        {
            try
            {
                var command = new SubmitBidCommand
                {
                    RfqId = request.RfqId,
                    UserId = request.UserId,
                    Proposal = request.Proposal,
                    CostOfProduct = request.CostOfProduct,
                    Discount = request.Discount,
                    SubmissionDate = DateTime.Now,
                    CostOfShipping = request.CostOfShipping,
                    Status = request.Status,
                  



                };

                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting bid for RFQ: {RfqId}", request.RfqId);
                return StatusCode(500, new { message = "Error submitting bid" });
            }
        }

        [HttpPut("{id}/status")]
        [Authorize(Policy = "RequirePermission")]
        public async Task<ActionResult> UpdateBidStatus(Guid id, [FromBody] UpdateBidStatusRequest request)
        {
            try
            {
                var command = new UpdateBidStatusCommand
                {
                    BidId = id,
                    Status = request.Status,
                    Notes = request.Notes
                };

                var result = await _mediator.Send(command);
                if (!result)
                    return NotFound();

                return Ok(new { message = "Bid status updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating bid status. BidId: {BidId}, Status: {Status}", id, request.Status);
                return StatusCode(500, new { message = "Error updating bid status" });
            }
        }
    }

   
}