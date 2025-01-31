using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace SharedLibrary.MessageBroker
{
    public static class RabbitMQExtensions
    {
        public static IServiceCollection AddSharedRabbitMQ(this IServiceCollection services)
        {
            services.AddSingleton<IConnectionFactory>(sp =>
            {
                return new ConnectionFactory
                {
                    HostName = "localhost", // Configure from appsettings.json in production
                    Port = 5672,
                    UserName = "guest",
                    Password = "guest"
                };
            });

            services.AddSingleton<IConnection>(sp =>
            {
                var factory = sp.GetRequiredService<IConnectionFactory>();
                return factory.CreateConnection();
            });

            services.AddSingleton<IModel>(sp =>
            {
                var connection = sp.GetRequiredService<IConnection>();
                var channel = connection.CreateModel();

                // Declare exchange and queues
                channel.ExchangeDeclare("notification_exchange", ExchangeType.Direct, durable: true);
                channel.QueueDeclare("email_queue", durable: true, exclusive: false, autoDelete: false);
                channel.QueueBind("email_queue", "notification_exchange", "email");

                return channel;
            });

            return services;
        }
    }
}