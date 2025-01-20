namespace Dashboard_Analytics_System.Domain.Entities
{
    public class Dashboard
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public int RFQsCreated { get; set; }
        public int BidsSubmitted { get; set; }
        public int NotificationsCount { get; set; }

    }
}
