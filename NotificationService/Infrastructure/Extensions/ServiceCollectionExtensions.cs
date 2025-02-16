using Microsoft.Extensions.DependencyInjection;
using NotificationService.Infrastructure.MessageConsumers;

namespace NotificationService.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMessageConsumers(this IServiceCollection services)
        {
            services.AddScoped<NotificationConsumer>();
            services.AddScoped<BidEventConsumer>();
            services.AddScoped<RfqEventConsumer>();
            services.AddScoped<UserVerificationStatusConsumer>();
            services.AddScoped<OnboardingReminderConsumer>();
            services.AddScoped<EmailNotificationConsumer>();

            return services;
        }
    }
}
