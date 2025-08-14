using Microsoft.Extensions.DistributedMessage4RabbitMQ.Configuration;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    public sealed class RabbitMQConnectionProvider : IAsyncDisposable
    {
        private readonly RabbitMQOptions _options;
        private readonly Lazy<Task<IConnection>> _connectionLazy;
        private readonly SemaphoreSlim _channelLock = new(1, 1);
        public RabbitMQConnectionProvider(IOptions<RabbitMQOptions> optionsAccessor)
        {
            _options = optionsAccessor.Value;

            _connectionLazy = new Lazy<Task<IConnection>>(() =>
                Task.Run(() => CreateConnection()));
        }

        private IConnection CreateConnection()
        {
            var factory = new ConnectionFactory
            {
                UserName = _options.UserName,
                Password = _options.Password,
                VirtualHost = _options.VirtualHost,
                HostName = _options.HostName,
                Port = _options.Port,
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(5)
            };

            return factory.CreateConnection();
        }

        public async Task<IModel> CreateChannelAsync(CancellationToken cancellationToken = default)
        {
            // 等待连接创建完成
            var connection = await _connectionLazy.Value.ConfigureAwait(false);

            await _channelLock.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                return connection.CreateModel();
            }
            finally
            {
                _channelLock.Release();
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_connectionLazy.IsValueCreated)
            {
                var connection = await _connectionLazy.Value.ConfigureAwait(false);
                if (connection.IsOpen)
                    connection.Close();
                connection.Dispose();
            }
        }
    }
}
