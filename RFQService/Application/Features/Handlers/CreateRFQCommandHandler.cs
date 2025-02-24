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
                    _logger.LogWarning("Unauthorized access attempt - User not authenticated");
                    throw new UnauthorizedAccessException("User not authenticated.");
                }

                _logger.LogInformation(
                    "Creating RFQ. Title: {Title}, Company: {Company}, CreatedBy: {UserId}",
                    request.ContractTitle,
                    request.CompanyName,
                    userId);

                // Validate recipients
                ValidateRecipients(request.RecipientEmails);

                // custom mapping to create RFQ
                var rfq = request.ToRFQ();

                var createdRFQ = await _rfqRepository.AddAsync(rfq);

                _logger.LogInformation(
                    "RFQ created successfully. ID: {RfqId}, Title: {Title}, Recipients: {RecipientCount}",
                    createdRFQ.Id,
                    createdRFQ.ContractTitle,
                    createdRFQ.Recipients?.Count ?? 0);

                return createdRFQ.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error creating RFQ. Title: {Title}, Error: {Error}",
                    request.ContractTitle,
                    ex.Message);
                throw;
            }
        }

        private void ValidateRecipients(List<string>? emails)
        {
            if (emails == null || !emails.Any())
            {
                return;
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