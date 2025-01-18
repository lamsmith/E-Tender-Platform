using MediatR;
using RFQService.Application.Common.Interface.Repositories;
using RFQService.Application.Features.Queries;
using RFQService.Domain.Entities;
using RFQService.Domain.Paging;

namespace RFQService.Application.Features.Handlers
{
    public class GetAllRFQsQueryHandler : IRequestHandler<GetAllRFQsQuery, PaginatedList<RFQ>>
    {
        private readonly IRFQRepository _rfqRepository;

        public GetAllRFQsQueryHandler(IRFQRepository rfqRepository)
        {
            _rfqRepository = rfqRepository;
        }

        public async Task<PaginatedList<RFQ>> Handle(GetAllRFQsQuery request, CancellationToken cancellationToken)
        {
            return await _rfqRepository.GetAllAsync(request.PageRequest);
        }
    }
}
