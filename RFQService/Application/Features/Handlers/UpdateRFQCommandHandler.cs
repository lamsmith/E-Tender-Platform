using MediatR;
using RFQService.Application.Common.Interface.Repositories;
using RFQService.Application.Features.Commands;
using RFQService.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using MassTransit;
using SharedLibrary.Models.Messages.RfqEvents;

namespace RFQService.Application.Features.Handlers
{
    public class UpdateRFQCommandHandler : IRequestHandler<UpdateRFQCommand, bool>
    {
        private readonly IRFQRepository _rfqRepository;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<UpdateRFQCommandHandler> _logger;

        public UpdateRFQCommandHandler(
            IRFQRepository rfqRepository,
            IPublishEndpoint publishEndpoint,
            ILogger<UpdateRFQCommandHandler> logger)
        {
            _rfqRepository = rfqRepository;
            _publishEndpoint = publishEndpoint;
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

                // Validate recipients
                ValidateRecipients(request.RecipientEmails);

                // Update RFQ properties
                existingRFQ.ContractTitle = request.ContractTitle;
                existingRFQ.CompanyName = request.CompanyName;
                existingRFQ.ScopeOfSupply = request.ScopeOfSupply;
                existingRFQ.PaymentTerms = request.PaymentTerms;
                existingRFQ.DeliveryTerms = request.DeliveryTerms;
                existingRFQ.OtherInformation = request.OtherInformation;
                existingRFQ.Status = request.Status;
                existingRFQ.Deadline = request.Deadline;
                existingRFQ.Visibility = request.Visibility;

                // Update recipients
                existingRFQ.Recipients.Clear();
                existingRFQ.Recipients = request.RecipientEmails.Select(email => new RFQRecipient
                {
                    RFQId = existingRFQ.Id,
                    Email = email
                }).ToList();

                var updatedRFQ = await _rfqRepository.UpdateAsync(existingRFQ);
                var isSuccess = updatedRFQ != null;

                if (isSuccess)
                {
                    await _publishEndpoint.Publish(new RfqUpdatedMessage
                    {
                        RfqId = existingRFQ.Id,
                        ContractTitle = existingRFQ.ContractTitle,
                        Visibility = existingRFQ.Visibility.ToString(),
                        UpdatedAt = DateTime.UtcNow,
                        Deadline = existingRFQ.Deadline,
                        RecipientEmails = existingRFQ.Recipients.Select(r => r.Email).ToList()
                    }, cancellationToken);

                    _logger.LogInformation("RFQ updated successfully. RFQ ID: {RfqId}", existingRFQ.Id);
                }

                return isSuccess;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating RFQ: {RfqId}", request.Id);
                throw;
            }
        }

        private void ValidateRecipients(List<string> emails)
        {
            if (!emails.Any())
            {
                throw new ValidationException("At least one recipient email is required.");
            }

            foreach (var email in emails)
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    throw new ValidationException("Email address cannot be empty.");
                }

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
