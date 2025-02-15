namespace SharedLibrary.Models.Messages.UserEvents
{
    public class OnboardingReminderMessage
    {
        public Guid UserId { get; set; }
        public string UserEmail { get; set; }
        public DateTime CreatedAt { get; set; }
        public int ReminderCount { get; set; }
    }
}