using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DistributedMessage4RabbitMQ.Configuration;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    public class RabbitMQConnectionProvider
    {
        protected SemaphoreSlim ConnectionLock { get; }
        protected RabbitMQOptions Options { get; }
        public RabbitMQConnectionProvider(IOptions<RabbitMQOptions> optionsAccessor)
        {
            Options = optionsAccessor.Value;
            ConnectionLock = new(initialCount: 1, maxCount: 1);
        }

        public virtual async Task<IConnection> GetConnectionAsync(CancellationToken cancellationToken)
        {
            IConnection connection;
            await ConnectionLock.WaitAsync(cancellationToken);
            try
            {
                ConnectionFactory factory = new()
                {
                    UserName = Options.UserName,
                    Password = Options.Password,
                    VirtualHost = Options.VirtualHost,
                    HostName = Options.HostName,
                    Port = Options.Port
                };
                connection = factory.CreateConnection();
            }
            finally
            {
                ConnectionLock.Release();
            }
            return connection;
        }
    }
}
