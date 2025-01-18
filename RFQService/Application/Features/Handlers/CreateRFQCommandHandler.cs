using MediatR;
using RFQService.Application.Common.Interface.Repositories;
using RFQService.Application.Features.Commands;
using RFQService.Domain.Entities;
using System.Security.Claims;

namespace RFQService.Application.Features.Handlers
{
    public class CreateRFQCommandHandler : IRequestHandler<CreateRFQCommand, Guid>
    {
        private readonly IRFQRepository _rfqRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CreateRFQCommandHandler(IRFQRepository rfqRepository, IHttpContextAccessor httpContextAccessor)
        {
            _rfqRepository = rfqRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Guid> Handle(CreateRFQCommand request, CancellationToken cancellationToken)
        {
            // Assuming you have a way to get the current user's ID, e.g., from HttpContext
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("User not authenticated.");
            }

            var rfq = new RFQ
            {
                ContractTitle = request.RFQData.ContractTitle,
                // Map other RFQ properties
                CreatedByUserId = Guid.Parse(userId), // Convert string ID to Guid if needed
                CreatedAt = DateTime.UtcNow
            };

            var createdRFQ = await _rfqRepository.AddAsync(rfq);

            // Handle documents if they are part of the request
            foreach (var docRequest in request.RFQData.Documents)
            {
                var document = new RFQDocument
                {
                    RFQId = createdRFQ.Id,
                    FileName = docRequest.Name,
                    FileType = docRequest.ContentType,
                    FileUrl = docRequest.FileUrl,
                    UploadedAt = DateTime.UtcNow
                     
                };

                await _rfqRepository.AddDocumentAsync(document);
            }

            return createdRFQ.Id;
        }
    }
}
