using NotificationService.Infrastructure.Extensions;
using SharedLibrary.MessageBroker.Interfaces;

namespace NotificationService.Infrastructure.Services
{
    public class MessageSubscriberService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly MessageSubscriberOptions _options;
        private readonly ILogger<MessageSubscriberService> _logger;

        public MessageSubscriberService(
            IServiceProvider serviceProvider,
            MessageSubscriberOptions options,
            ILogger<MessageSubscriberService> logger)
        {
            _serviceProvider = serviceProvider;
            _options = options;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var messageConsumer = scope.ServiceProvider.GetRequiredService<IMessageConsumer>();

                foreach (var registration in _options.ConsumerRegistrations)
                {
                    _logger.LogInformation(
                        "Starting consumer {ConsumerType} for queue {QueueName}",
                        registration.ConsumerType.Name,
                        registration.QueueName);

                    // Start consuming messages
                    await messageConsumer.StartConsumingAsync(registration.QueueName, stoppingToken);
                }

                // Keep the service running
                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in MessageSubscriberService");
                throw;
            }
        }
    }
}