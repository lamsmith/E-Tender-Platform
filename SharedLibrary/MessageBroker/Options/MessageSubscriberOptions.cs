using SharedLibrary.MessageBroker.Interfaces;

namespace SharedLibrary.MessageBroker.Options
{
    public class MessageSubscriberOptions
    {
        public List<ConsumerRegistration> ConsumerRegistrations { get; } = new();

        public void AddConsumer<TConsumer>(string queueName) where TConsumer : IMessageConsumer
        {
            ConsumerRegistrations.Add(new ConsumerRegistration
            {
                QueueName = queueName,
                ConsumerType = typeof(TConsumer)
            });
        }
    }

    public class ConsumerRegistration
    {
        public string QueueName { get; set; }
        public Type ConsumerType { get; set; }
    }
}