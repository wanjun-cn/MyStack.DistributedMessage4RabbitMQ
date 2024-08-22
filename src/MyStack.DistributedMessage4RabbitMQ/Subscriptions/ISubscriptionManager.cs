using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ.Subscriptions
{
    internal interface ISubscriptionManager
    {
        IList<SubscriptionInfo>? GetSubscriptions(Type eventType);
        IList<SubscriptionInfo>? GetAllSubscriptions();
    }
}
