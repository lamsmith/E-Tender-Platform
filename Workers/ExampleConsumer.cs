using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using YourNamespace.Infrastructure.MessageBroker;

public class ExampleConsumer : BackgroundService
{
    private readonly RabbitMQConsumer<dynamic> _consumer;

    public ExampleConsumer(RabbitMQConnection connection)
    {
        _consumer = new RabbitMQConsumer<dynamic>(connection, "example-queue");
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _consumer.Consume(message =>
        {
            // Handle the message here
            Console.WriteLine($"Received message: {message}");
        });

        return Task.CompletedTask;
    }
}