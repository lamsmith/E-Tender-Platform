using MediatR;
using RFQService.Application.Common.Interface.Repositories;
using RFQService.Application.Features.Queries;
using RFQService.Domain.Entities;

namespace RFQService.Application.Features.Handlers
{
    public class GetRFQByIdQueryHandler : IRequestHandler<GetRFQByIdQuery, RFQ>
    {
        private readonly IRFQRepository _rfqRepository;

        public GetRFQByIdQueryHandler(IRFQRepository rfqRepository)
        {
            _rfqRepository = rfqRepository;
        }

        public async Task<RFQ> Handle(GetRFQByIdQuery request, CancellationToken cancellationToken)
        {
            return await _rfqRepository.GetByIdAsync(request.RFQId);
        }
    }
}
