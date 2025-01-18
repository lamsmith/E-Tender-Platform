using MediatR;
using RFQService.Application.Common.Interface.Repositories;
using RFQService.Application.Features.Commands;
using RFQService.Domain.Enums;

namespace RFQService.Application.Features.Handlers
{
    public class CloseRFQCommandHandler : IRequestHandler<CloseRFQCommand, bool>
    {
        private readonly IRFQRepository _rfqRepository;

        public CloseRFQCommandHandler(IRFQRepository rfqRepository)
        {
            _rfqRepository = rfqRepository;
        }

        public async Task<bool> Handle(CloseRFQCommand request, CancellationToken cancellationToken)
        {
            var rfq = await _rfqRepository.GetByIdAsync(request.RFQId);
            if (rfq == null) return false;

            rfq.Status = Status.Closed;
            await _rfqRepository.UpdateAsync(rfq);
            return true;
        }
    }
}
