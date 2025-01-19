using MediatR;
using RFQService.Application.Common.Interface.Repositories;
using RFQService.Application.Features.Commands;
using RFQService.Domain.Entities;
using RFQService.Application.Extensions; // Include this for your mapping extensions
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
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("User not authenticated.");
            }

            // Use custom mapping to create RFQ
            var rfq = request.RFQData.ToRFQ();
            rfq.CreatedByUserId = Guid.Parse(userId); 

            var createdRFQ = await _rfqRepository.AddAsync(rfq);

            // Handle documents if they are part of the request
            foreach (var docRequest in request.RFQData.Documents)
            {
                // Use custom mapping for document creation
                var document = docRequest.ToRFQDocument();
                document.RFQId = createdRFQ.Id; 
                document.UploadedAt = DateTime.UtcNow;

                await _rfqRepository.AddDocumentAsync(document);
            }

            return createdRFQ.Id;
        }
    }
}