using Microsoft.Extensions.DependencyInjection;
using NotificationService.MessageBroker;
using NotificationService.Infrastructure.Services;
using SharedLibrary.MessageBroker.Interfaces;

namespace NotificationService.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMessageConsumer<TConsumer>(
            this IServiceCollection services,
            string queueName) where TConsumer : class, IMessageConsumer
        {
            services.AddScoped<TConsumer>();
            services.Configure<MessageSubscriberOptions>(options =>
            {
                options.AddConsumer<TConsumer>(queueName);
            });

            return services;
        }

        public static IServiceCollection AddMessageSubscriber(
            this IServiceCollection services,
            Action<MessageSubscriberOptions> configureOptions)
        {
            var options = new MessageSubscriberOptions();
            configureOptions(options);

            services.Configure<MessageSubscriberOptions>(config =>
            {
                foreach (var registration in options.ConsumerRegistrations)
                {
                    config.ConsumerRegistrations.Add(registration);
                }
            });

            services.AddHostedService<MessageSubscriberService>();

            return services;
        }
    }

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
