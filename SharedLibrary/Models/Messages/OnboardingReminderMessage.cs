namespace SharedLibrary.Models.Messages
{
    public class OnboardingReminderMessage
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public List<string> IncompleteTasks { get; set; }
        public int DaysRegistered { get; set; }
        public DateTime LastLoginAt { get; set; }
    }
}