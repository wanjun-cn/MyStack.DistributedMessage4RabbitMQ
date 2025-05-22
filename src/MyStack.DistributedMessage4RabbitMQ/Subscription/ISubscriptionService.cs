using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ.Subscription
{
    public interface ISubscriptionService
    {
        IReadOnlyList<Type>? GetMessageTypes(string routingKey);
        IReadOnlyList<Type>? GetAllMessageTypes();
        bool IsEvent(string routingKey);
        bool IsRpcRequest(string routingKey);
    }
}
