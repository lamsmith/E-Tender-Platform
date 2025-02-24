using MediatR;
using RFQService.Application.Common.Interface.Repositories;
using RFQService.Application.Features.Commands;
using RFQService.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace RFQService.Application.Features.Handlers
{
    public class UpdateRFQCommandHandler : IRequestHandler<UpdateRFQCommand, bool>
    {
        private readonly IRFQRepository _rfqRepository;
        private readonly IHttpContextAccessor _httpContextAccessor; // To access the current user's claims
        private readonly ILogger<UpdateRFQCommandHandler> _logger;

        public UpdateRFQCommandHandler(
            IRFQRepository rfqRepository,
            IHttpContextAccessor httpContextAccessor,
            ILogger<UpdateRFQCommandHandler> logger)
        {
            _rfqRepository = rfqRepository;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<bool> Handle(UpdateRFQCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var existingRFQ = await _rfqRepository.GetByIdAsync(request.Id);
                if (existingRFQ == null)
                {
                    throw new KeyNotFoundException($"RFQ with ID {request.Id} not found.");
                }

               
                var userId = GetCurrentUserId();
                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out Guid parsedUserId) || existingRFQ.CreatedByUserId != parsedUserId)
                {
                    _logger.LogWarning("User {UserId} is not authorized to update RFQ {RfqId}", userId, request.Id);
                    throw new UnauthorizedAccessException("Only the creator of the RFQ can update it.");
                }

                
                if (request.RecipientEmails != null && request.RecipientEmails.Any())
                {
                    ValidateRecipients(request.RecipientEmails);
                }

                
                existingRFQ.ContractTitle = request.ContractTitle;
                existingRFQ.CompanyName = request.CompanyName;
                existingRFQ.ScopeOfSupply = request.ScopeOfSupply;
                existingRFQ.PaymentTerms = request.PaymentTerms;
                existingRFQ.DeliveryTerms = request.DeliveryTerms;
                existingRFQ.OtherInformation = request.OtherInformation;
                existingRFQ.Status = request.Status;
                existingRFQ.Deadline = request.Deadline;
                existingRFQ.Visibility = request.Visibility;
                existingRFQ.Recipients = new List<RFQRecipient>();


                if (request.RecipientEmails != null && request.RecipientEmails.Any())
                {
                    existingRFQ.Recipients.Clear();
                    existingRFQ.Recipients = request.RecipientEmails.Select(email => new RFQRecipient
                    {
                        RFQId = existingRFQ.Id,
                        Email = email
                    }).ToList();
                }

                var updatedRFQ = await _rfqRepository.UpdateAsync(existingRFQ);
                var isSuccess = updatedRFQ != null;

                return isSuccess;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating RFQ: {RfqId}", request.Id);
                throw;
            }
        }

        private string GetCurrentUserId()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        private void ValidateRecipients(List<string> emails)
        {
            foreach (var email in emails.Where(e => !string.IsNullOrWhiteSpace(e)))
            {
                if (!IsValidEmail(email))
                {
                    throw new ValidationException($"Invalid email format: {email}");
                }
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}