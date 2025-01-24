using System;
using YourNamespace.Infrastructure.MessageBroker;

public class ExampleService
{
    private readonly IMessagePublisher _messagePublisher;
    private const string QueueName = "example-queue";

    public ExampleService(IMessagePublisher messagePublisher)
    {
        _messagePublisher = messagePublisher;
    }

    public void SendMessage(string message)
    {
        var messageObject = new { Content = message, Timestamp = DateTime.UtcNow };
        _messagePublisher.PublishMessage(QueueName, messageObject);
    }
}