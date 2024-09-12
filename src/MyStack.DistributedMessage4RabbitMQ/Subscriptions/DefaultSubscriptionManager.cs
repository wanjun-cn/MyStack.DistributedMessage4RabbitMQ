using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ.Subscriptions
{
    internal class DefaultSubscriptionManager : ISubscriptionManager, ISubscriptionRegistrar
    {
        private static Dictionary<string, List<SubscriptionInfo>> _subscriptions = new Dictionary<string, List<SubscriptionInfo>>();
        private readonly IRoutingKeyResolver _routingKeyResolver;
        public DefaultSubscriptionManager(IRoutingKeyResolver routingKeyResolver)
        {
            _routingKeyResolver = routingKeyResolver;
        }
        public Dictionary<string, List<SubscriptionInfo>>? GetAllSubscriptions()
        {
            return _subscriptions;
        }
        public IList<SubscriptionInfo>? GetSubscriptions(string messageKey)
        {
            if (_subscriptions.TryGetValue(messageKey, out var subscriptions))
                return subscriptions;
            return null;
        }
        public IList<SubscriptionInfo>? GetSubscriptions(Type messageType)
        {
            if (_subscriptions.TryGetValue(messageType.FullName, out var subscriptions))
                return subscriptions;
            return null;
        }
        public void Register(List<SubscriptionInfo> subscriptions)
        {
            if (subscriptions == null)
                return;
            foreach (var subscription in subscriptions)
            {
                string messageKey;
                if (subscription.MessageType != null)
                    messageKey = _routingKeyResolver.GetRoutingKey(subscription.MessageType);
                else if (!string.IsNullOrEmpty(subscription.MessageKey))
                    messageKey = subscription.MessageKey;
                else
                    throw new InvalidOperationException("订阅的键不能为空");

                var messageType = subscription.MessageType;
                if (!_subscriptions.ContainsKey(messageKey))
                {
                    _subscriptions[messageKey] = new List<SubscriptionInfo>();
                }
                _subscriptions[messageKey].Add(subscription);
            }
        }
    }
}
