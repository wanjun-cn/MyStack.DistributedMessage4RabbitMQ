using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    public class RabbitMQProvider
    {
        private readonly SemaphoreSlim _connectionLock = new(initialCount: 1, maxCount: 1);
        private readonly RabbitMQOptions _options;
        public RabbitMQProvider(IOptions<RabbitMQOptions> optionsAccessor)
        {
            _options = optionsAccessor.Value;
        }

        public async Task<IConnection> GetConnectionAsync(CancellationToken cancellationToken)
        {
            IConnection connection;
            await _connectionLock.WaitAsync(cancellationToken);
            try
            {
                ConnectionFactory factory = new()
                {
                    UserName = _options.UserName,
                    Password = _options.Password,
                    VirtualHost = _options.VirtualHost,
                    HostName = _options.HostName,
                    Port = _options.Port
                };
                connection = factory.CreateConnection();
            }
            finally
            {
                _connectionLock.Release();
            }
            return connection;
        }
    }
}
