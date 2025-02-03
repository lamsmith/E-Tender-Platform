using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Backoffice_Services.Application.Features.RFQManagement.Commands;
using Backoffice_Services.Application.Features.RFQManagement.Queries;
using Backoffice_Services.Application.DTO.RFQManagement.Requests;
using Backoffice_Services.Application.DTO.RFQManagement.Responses;
using RFQService.Domain.Paging;

namespace Backoffice_Services.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class RFQManagementController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<RFQManagementController> _logger;

        public RFQManagementController(
            IMediator mediator,
            ILogger<RFQManagementController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetRFQs(
            [FromQuery] string? status,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var query = new GetRFQsQuery
                {
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
                _logger.LogError(ex, "Error retrieving RFQs");
                return StatusCode(500, new { message = "Error retrieving RFQs" });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRFQById(Guid id)
        {
            try
            {
                var rfq = await _mediator.Send(new GetRFQByIdQuery { Id = id });
                if (rfq == null)
                    return NotFound(new { message = $"RFQ with ID {id} not found" });

                return Ok(rfq);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving RFQ: {RFQId}", id);
                return StatusCode(500, new { message = "Error retrieving RFQ" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateRFQ([FromBody] RFQCreationRequestModel request)
        {
            try
            {
                var command = new CreateRFQCommand
                {
                    ContractTitle = request.ContractTitle,
                    ScopeOfSupply = request.ScopeOfSupply,
                    PaymentTerms = request.PaymentTerms,
                    DeliveryTerms = request.DeliveryTerms,
                    OtherInformation = request.OtherInformation,
                    Deadline = request.Deadline,
                    Visibility = request.Visibility,
                    CreatedByUserId = request.CreatedByUserId,
                    Documents = request.Documents?.Select(doc => new RFQDocumentModel
                    {
                        FileName = doc.Name,
                        FileType = doc.ContentType,
                        FileUrl = doc.FileUrl
                    }).ToList() ?? new List<RFQDocumentModel>()
                };

                var result = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetRFQById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating RFQ: {@Request}", request);
                return StatusCode(500, new { message = "Error creating RFQ" });
            }
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateRFQStatus(Guid id, [FromBody] UpdateRFQStatusRequest request)
        {
            try
            {
                var command = new UpdateRFQStatusCommand
                {
                    RFQId = id,
                    Status = request.Status
                };

                var result = await _mediator.Send(command);

                if (!result)
                    return NotFound(new { message = $"RFQ with ID {id} not found" });

                return Ok(new { message = "RFQ status updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating RFQ status: {RFQId}, {@Request}", id, request);
                return StatusCode(500, new { message = "Error updating RFQ status" });
            }
        }
    }
}
