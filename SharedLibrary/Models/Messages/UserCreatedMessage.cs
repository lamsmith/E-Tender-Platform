namespace SharedLibrary.Models.Messages
{
    public class UserCreatedMessage
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CompanyName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Industry { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ProfileCompletedAt { get; set; }
    }
}