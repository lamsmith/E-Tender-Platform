using MassTransit;
using Microsoft.Extensions.Logging;

namespace SharedLibrary.MessageBroker.Consumers
{
    public abstract class BaseConsumer<TMessage> : IConsumer<TMessage> where TMessage : class
    {
        protected readonly ILogger _logger;

        protected BaseConsumer(ILogger logger)
        {
            _logger = logger;
        }

        public abstract Task Consume(ConsumeContext<TMessage> context);
    }
}