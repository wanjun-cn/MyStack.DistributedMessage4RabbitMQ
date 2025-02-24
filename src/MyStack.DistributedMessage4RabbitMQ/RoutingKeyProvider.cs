using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    /// <summary>
    /// Represents the service for providing routing keys.
    /// </summary>
    public class RoutingKeyProvider
    {
        private readonly RabbitMQOptions _options;
        public RoutingKeyProvider(IOptions<RabbitMQOptions> optionsAccessor)
        {
            _options = optionsAccessor.Value;
        }
        public string GetValue(string messageName)
        {
            return _options.RoutingKeyPrefix + messageName;
        }
    }
}
