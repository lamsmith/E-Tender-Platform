using Dashboard_Analytics_System.Application.DTO;

namespace Dashboard_Analytics_System.Application.Common.Inteface
{
    public interface IDashboardService
    {
        Task<RFQDashboardDTO> GetRFQDashboardAsync();
        Task<BidDashboardDTO> GetBidDashboardAsync();
    }
}
