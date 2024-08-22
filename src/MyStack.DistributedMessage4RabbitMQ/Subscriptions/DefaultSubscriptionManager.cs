using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ.Subscriptions
{
    internal class DefaultSubscriptionManager : ISubscriptionManager, ISubscriptionRegistrar
    {
        private static Dictionary<Type, List<SubscriptionInfo>> _subscriptions = new Dictionary<Type, List<SubscriptionInfo>>();
        public IList<SubscriptionInfo>? GetAllSubscriptions()
        {
            return _subscriptions.Values.SelectMany(x => x).ToList();
        }

        public IList<SubscriptionInfo>? GetSubscriptions(Type eventType)
        {
            if (_subscriptions.TryGetValue(eventType, out var subscriptions))
                return subscriptions;
            return null;
        }
        public void Register(List<SubscriptionInfo> subscriptions)
        {
            if (subscriptions == null)
                return;
            foreach (var subscription in subscriptions)
            {
                var eventType = subscription.EventType;
                if (!_subscriptions.ContainsKey(subscription.EventType))
                {
                    _subscriptions[eventType] = new List<SubscriptionInfo>();
                }
                _subscriptions[eventType].Add(subscription);
            }
        }
    }
}
