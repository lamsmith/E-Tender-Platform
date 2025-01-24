using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace SharedLibrary.MessageBroker
{
    public class RabbitMQConnection : IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private bool _disposed;

        public RabbitMQConnection(IConfiguration configuration)
        {
            var factory = new ConnectionFactory
            {
                HostName = configuration["RabbitMQ:HostName"] ?? "localhost",
                Port = int.Parse(configuration["RabbitMQ:Port"] ?? "5672"),
                UserName = configuration["RabbitMQ:UserName"] ?? "guest",
                Password = configuration["RabbitMQ:Password"] ?? "guest",
                DispatchConsumersAsync = true
            };

            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
            }
            catch (Exception ex)
            {
                throw new Exception("Could not connect to RabbitMQ", ex);
            }
        }

        public IModel Channel => _channel;

        public void Dispose()
        {
            if (_disposed) return;

            try
            {
                if (_channel?.IsOpen ?? false)
                {
                    _channel?.Close();
                    _channel?.Dispose();
                }

                if (_connection?.IsOpen ?? false)
                {
                    _connection?.Close();
                    _connection?.Dispose();
                }
            }
            catch (Exception)
            {
                // Log exception here
            }

            _disposed = true;
        }
    }
}