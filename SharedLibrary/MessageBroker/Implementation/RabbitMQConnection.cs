using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.Logging;
using SharedLibrary.MessageBroker.Interfaces;

namespace SharedLibrary.MessageBroker.Implementation
{
    public class RabbitMQConnection : IRabbitMQConnection
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly ILogger<RabbitMQConnection> _logger;
        private IConnection _connection;
        private bool _disposed;

        private readonly object sync_root = new object();

        public RabbitMQConnection(IConnectionFactory connectionFactory, ILogger<RabbitMQConnection> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        public bool IsConnected => _connection != null && _connection.IsOpen && !_disposed;

        public IConnection CreateConnection()
        {
            if (!IsConnected)
            {
                TryConnect();
            }

            return _connection;
        }

        public bool TryConnect()
        {
            _logger.LogInformation("RabbitMQ Client is trying to connect");

            lock (sync_root)
            {
                if (IsConnected) return true;

                try
                {
                    _connection = _connectionFactory.CreateConnection();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error connecting to RabbitMQ");
                    return false;
                }

                if (IsConnected)
                {
                    _connection.ConnectionShutdown += OnConnectionShutdown;
                    _connection.CallbackException += OnCallbackException;
                    _connection.ConnectionBlocked += OnConnectionBlocked;

                    _logger.LogInformation(
                        "RabbitMQ Client acquired a persistent connection to '{HostName}'",
                        _connection.Endpoint.HostName);

                    return true;
                }

                return false;
            }
        }

        private void OnConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
        {
            if (_disposed) return;

            _logger.LogWarning("A RabbitMQ connection is shutdown. Trying to re-connect...");
            TryConnect();
        }

        private void OnCallbackException(object sender, CallbackExceptionEventArgs e)
        {
            if (_disposed) return;

            _logger.LogWarning("A RabbitMQ connection throw exception. Trying to re-connect...");
            TryConnect();
        }

        private void OnConnectionShutdown(object sender, ShutdownEventArgs reason)
        {
            if (_disposed) return;

            _logger.LogWarning("A RabbitMQ connection is on shutdown. Trying to re-connect...");
            TryConnect();
        }

        public void Dispose()
        {
            if (_disposed) return;

            _disposed = true;

            try
            {
                _connection.ConnectionShutdown -= OnConnectionShutdown;
                _connection.CallbackException -= OnCallbackException;
                _connection.ConnectionBlocked -= OnConnectionBlocked;
                _connection.Dispose();
            }
            catch (IOException ex)
            {
                _logger.LogCritical(ex, "Error disposing RabbitMQ connection");
            }
        }
    }
}