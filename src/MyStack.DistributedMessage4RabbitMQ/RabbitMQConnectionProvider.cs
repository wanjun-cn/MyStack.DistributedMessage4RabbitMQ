using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.DistributedMessage4RabbitMQ.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    public delegate Task NewChannelCreatedEventHandler(IModel channel, CancellationToken cancellationToken);
    public class RabbitMQConnectionProvider : IDisposable
    {
        protected SemaphoreSlim ConnectionLock { get; }
        protected RabbitMQOptions Options { get; }
        protected ILogger<RabbitMQConnectionProvider> Logger { get; }
        public event NewChannelCreatedEventHandler? OnNewChannelCreated;
        public RabbitMQConnectionProvider(IOptions<RabbitMQOptions> optionsAccessor,
            ILogger<RabbitMQConnectionProvider> logger)
        {
            Options = optionsAccessor.Value;
            ConnectionLock = new(initialCount: 1, maxCount: 1);
            Logger = logger;
        }
        private IConnection? _connection;
        private IModel? _channel;
        public virtual async Task<IModel> CreateChannelAsync(CancellationToken cancellationToken)
        {
            await ConnectionLock.WaitAsync(cancellationToken);
            try
            {
                _connection = CreateConnection();
                _connection.ConnectionShutdown += OnConnectionShutdown;
                _channel = _connection.CreateModel();
                _channel.ModelShutdown += OnChannel_ModelShutdown;
            }
            finally
            {
                ConnectionLock.Release();
            }
            return _channel;
        }
        protected virtual IConnection CreateConnection()
        {
            ConnectionFactory factory = new()
            {
                UserName = Options.UserName,
                Password = Options.Password,
                VirtualHost = Options.VirtualHost,
                HostName = Options.HostName,
                Port = Options.Port,
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(5)
            };
            return factory.CreateConnection();
        }
        protected virtual void OnConnectionShutdown(object? sender, ShutdownEventArgs e)
        {
            Logger.LogInformation("Connection shutdown due to: {Reason}. Attempting to re-establish connection.", e.ReplyText);
            if (_connection == null || !_connection.IsOpen)
            {
                try
                {
                    _connection = CreateConnection();
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Failed to close connection: {ex.Message}");
                }
            }

        }
        protected virtual async void OnChannel_ModelShutdown(object? sender, ShutdownEventArgs e)
        {
            Logger.LogInformation("Channel shutdown: {Reason}. Attempting to create a new channel.", e.ReplyText);
            if (_channel == null || _channel.IsClosed)
            {
                try
                {
                    _connection?.Dispose();
                    _connection = CreateConnection();
                    _channel = _connection.CreateModel();
                    OnNewChannelCreated?.Invoke(_channel, CancellationToken.None);
                }
                catch (Exception ex)
                {
                    Logger.LogError("Failed to reconnect channel: {Message}", ex.Message);
                }
            }
            await Task.CompletedTask;
        }
        public virtual void Dispose()
        {
            if (_connection != null)
                _connection.Dispose();
            if (_channel != null)
                _channel.Close();
        }
    }
}
