using Microsoft.Extensions.DistributedMessage4RabbitMQ.Configuration;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    /// <summary>
    /// Represents the service for providing routing keys.
    /// </summary>
    public class RoutingKeyProvider
    {
        protected RabbitMQOptions Options { get; }
        public RoutingKeyProvider(IOptions<RabbitMQOptions> optionsAccessor)
        {
            Options = optionsAccessor.Value;
        }
        public virtual string GetValue(string messageName)
        {
            return Options.RoutingKeyPrefix + messageName;
        }
    }
}
