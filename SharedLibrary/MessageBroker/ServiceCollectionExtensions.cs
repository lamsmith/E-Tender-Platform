using Microsoft.Extensions.DependencyInjection;

namespace SharedLibrary.MessageBroker
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRabbitMQ(this IServiceCollection services)
        {
            services.AddSingleton<RabbitMQConnection>();
            services.AddScoped<IMessagePublisher, RabbitMQPublisher>();

            return services;
        }
    }
}