using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MassTransit;

namespace NotificationService.Infrastructure.Services
{
    public class MessageSubscriberService : BackgroundService
    {
        private readonly IBus _bus;
        private readonly ILogger<MessageSubscriberService> _logger;

        public MessageSubscriberService(
            IBus bus,
            ILogger<MessageSubscriberService> logger)
        {
            _bus = bus;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogInformation("Message subscriber service is starting");

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

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Message subscriber service is stopping.");
            await base.StopAsync(cancellationToken);
        }
    }
}