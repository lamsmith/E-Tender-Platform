using MediatR;
using RFQService.Application.Common.Interface.Repositories;
using RFQService.Application.Features.Commands;

namespace RFQService.Application.Features.Handlers
{
    public class UpdateRFQCommandHandler : IRequestHandler<UpdateRFQCommand, bool>
    {
        private readonly IRFQRepository _rfqRepository;

        public UpdateRFQCommandHandler(IRFQRepository rfqRepository)
        {
            _rfqRepository = rfqRepository;
        }

        public async Task<bool> Handle(UpdateRFQCommand request, CancellationToken cancellationToken)
        {
            var rfq = await _rfqRepository.GetByIdAsync(request.RFQId);
            if (rfq == null) return false;

            // Update RFQ properties
            rfq.ContractTitle = request.UpdateData.ContractTitle ?? rfq.ContractTitle;
            // Map other properties...

            await _rfqRepository.UpdateAsync(rfq);
            return true;
        }
    }
}
