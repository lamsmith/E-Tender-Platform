using MediatR;
using RFQService.Application.Common.Interface.Repositories;
using RFQService.Application.Features.Commands;
using RFQService.Domain.Entities;
using RFQService.Application.Extensions;
using System.Security.Claims;
using MassTransit;
using SharedLibrary.Models.Messages.RfqEvents;
using System.ComponentModel.DataAnnotations;

namespace RFQService.Application.Features.Handlers
{
    public class CreateRFQCommandHandler : IRequestHandler<CreateRFQCommand, Guid>
    {
        private readonly IRFQRepository _rfqRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<CreateRFQCommandHandler> _logger;

        public CreateRFQCommandHandler(
            IRFQRepository rfqRepository,
            IHttpContextAccessor httpContextAccessor,
            IPublishEndpoint publishEndpoint,
            ILogger<CreateRFQCommandHandler> logger)
        {
            _rfqRepository = rfqRepository;
            _httpContextAccessor = httpContextAccessor;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task<Guid> Handle(CreateRFQCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    throw new UnauthorizedAccessException("User not authenticated.");
                }

                // Validate recipients
                ValidateRecipients(request.RecipientEmails);

                // Use custom mapping to create RFQ
                var rfq = request.ToRFQ();

                var createdRFQ = await _rfqRepository.AddAsync(rfq);

                //// Publish RFQ created event using MassTransit
                //await _publishEndpoint.Publish(new RfqCreatedMessage
                //{
                //    RfqId = createdRFQ.Id,
                //    ContractTitle = createdRFQ.ContractTitle,
                //    CompanyName = createdRFQ.CompanyName,
                //    ScopeOfSupply = createdRFQ.ScopeOfSupply,
                //    PaymentTerms = createdRFQ.PaymentTerms,
                //    DeliveryTerms = createdRFQ.DeliveryTerms,
                //    OtherInformation = createdRFQ.OtherInformation,
                //    CreatedByUserId = createdRFQ.CreatedByUserId,
                //    Visibility = createdRFQ.Visibility.ToString(),
                //    CreatedAt = createdRFQ.CreatedAt,
                //    Deadline = createdRFQ.Deadline,
                //  //  RecipientEmails = createdRFQ.Recipients.Select(r => r.Email).ToList()
                //}, cancellationToken);

                _logger.LogInformation("RFQ created and event published. RFQ ID: {RfqId}", createdRFQ.Id);

                return createdRFQ.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating RFQ: {Title}", request.ContractTitle);
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