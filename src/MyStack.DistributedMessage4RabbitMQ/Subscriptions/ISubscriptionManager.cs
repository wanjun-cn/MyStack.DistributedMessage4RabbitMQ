using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ.Subscriptions
{
    internal interface ISubscriptionManager
    {
        IList<SubscriptionInfo>? GetSubscriptions(string messageKey);
        IList<SubscriptionInfo>? GetSubscriptions(Type messageType);
        Dictionary<string, List<SubscriptionInfo>>? GetAllSubscriptions();
    }
}
