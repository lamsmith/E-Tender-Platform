//using Dashboard_Analytics_System.Application.Common.Inteface;
//using Dashboard_Analytics_System.Application.DTO;
//using MediatR;

//namespace Dashboard_Analytics_System.Infrastructure.Persistence.Services
//{
//    public class DashboardService : IDashboardService
//    {
//        private readonly IMediator _mediator;

//        public DashboardService(IMediator mediator)
//        {
//            _mediator = mediator;
//        }

//        public async Task<RFQDashboardDTO> GetRFQDashboardAsync()
//        {
//            return new RFQDashboardDTO
//            {
//                TotalRFQs = await _mediator.Send(new GetTotalRFQsQuery()),
//                OpenRFQs = await _mediator.Send(new CountRFQsByStatusQuery { Status = RFQStatus.Open }),
//                ClosedRFQs = await _mediator.Send(new CountRFQsByStatusQuery { Status = RFQStatus.Closed }),
       
//            };
//        }

//        public async Task<BidDashboardDTO> GetBidDashboardAsync()
//        {
//            return new BidDashboardDTO
//            {
//                TotalBids = await _mediator.Send(new GetTotalBidsQuery()),
//                PendingBids = await _mediator.Send(new CountBidsByStatusQuery { Status = BidStatus.Pending }),
//                AcceptedBids = await _mediator.Send(new CountBidsByStatusQuery { Status = BidStatus.Accepted }),
//                RejectedBids = await _mediator.Send(new CountBidsByStatusQuery { Status = BidStatus.Rejected }),
//                BidsThisMonth = await _mediator.Send(new GetBidsSubmittedThisMonthQuery()),
//                BidSuccessRate = await _mediator.Send(new CalculateBidSuccessRateQuery())
//            };
//        }
//    }
//}
