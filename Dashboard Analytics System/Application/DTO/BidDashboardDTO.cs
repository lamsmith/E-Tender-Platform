namespace Dashboard_Analytics_System.Application.DTO
{
    public class BidDashboardDTO
    {
        public int TotalBids { get; set; }
        public int PendingBids { get; set; }
        public int AcceptedBids { get; set; }
        public int RejectedBids { get; set; }
        public int BidsThisMonth { get; set; }
        public decimal BidSuccessRate { get; set; }
    }
}
