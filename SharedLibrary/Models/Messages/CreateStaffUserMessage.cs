namespace SharedLibrary.Models.Messages
{
    public class CreateStaffUserMessage
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Guid InitiatorUserId { get; set; }
    }

    public class CreateStaffUserResponse
    {
        public Guid UserId { get; set; }
    }
}