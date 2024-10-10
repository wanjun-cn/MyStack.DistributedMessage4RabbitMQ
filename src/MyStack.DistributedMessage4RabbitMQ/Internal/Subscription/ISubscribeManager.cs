using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ.Internal.Subscription
{
    internal interface ISubscribeManager
    {
        IList<SubscribeInfo>? GetSubscribers(string messageKey);
        IList<SubscribeInfo>? GetSubscribers(Type messageType);
        Dictionary<string, List<SubscribeInfo>>? GetAllSubscribers();
    }
}
