namespace SharedLibrary.Models.Messages
{
    public class KycSubmittedMessage
    {
        public Guid UserId { get; set; }
        public string CompanyName { get; set; }
        public string RcNumber { get; set; }
        public string CompanyAddress { get; set; }
        public string Industry { get; set; }
        public string PhoneNumber { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public DateTime SubmittedAt { get; set; }
    }
}