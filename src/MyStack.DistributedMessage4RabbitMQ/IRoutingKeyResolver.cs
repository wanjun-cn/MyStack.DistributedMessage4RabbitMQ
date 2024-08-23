using System;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    public interface IRoutingKeyResolver
    {
        string GetRoutingKey(Type messageType);
    }
}
