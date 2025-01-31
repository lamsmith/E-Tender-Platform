namespace SharedLibrary.Models.Messages
{
    public class StaffWelcomeEmailMessage
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string TempPassword { get; set; }
    }
}