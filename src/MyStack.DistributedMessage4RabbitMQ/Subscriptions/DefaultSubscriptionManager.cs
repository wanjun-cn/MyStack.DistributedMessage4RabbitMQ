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

        public IList<SubscriptionInfo>? GetSubscriptions(Type messageType)
        {
            if (_subscriptions.TryGetValue(messageType, out var subscriptions))
                return subscriptions;
            return null;
        }
        public void Register(List<SubscriptionInfo> subscriptions)
        {
            if (subscriptions == null)
                return;
            foreach (var subscription in subscriptions)
            {
                var messageType = subscription.MessageType;
                if (!_subscriptions.ContainsKey(subscription.MessageType))
                {
                    _subscriptions[messageType] = new List<SubscriptionInfo>();
                }
                _subscriptions[messageType].Add(subscription);
            }
        }
    }
}
