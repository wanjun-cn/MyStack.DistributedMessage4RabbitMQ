using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ.Internal.Subscription
{
    internal class SubscribeManager : ISubscribeManager, ISubscribeRegistrar
    {
        private static Dictionary<string, List<SubscribeInfo>> _subscriptions = new Dictionary<string, List<SubscribeInfo>>();
        private readonly IRoutingKeyResolver _routingKeyResolver;
        public SubscribeManager(IRoutingKeyResolver routingKeyResolver)
        {
            _routingKeyResolver = routingKeyResolver;
        }
        public Dictionary<string, List<SubscribeInfo>>? GetAllSubscribers()
        {
            return _subscriptions;
        }
        public IList<SubscribeInfo>? GetSubscribers(string messageKey)
        {
            // TODO:需优化查找消息方式
            var subscriptions = new List<SubscribeInfo>();
            foreach (var item in _subscriptions)
            {
                if (messageKey.Contains(item.Key.Replace("*.", "")))
                    subscriptions.AddRange(item.Value);
            }
            return subscriptions;
        }
        public IList<SubscribeInfo>? GetSubscribers(Type messageType)
        {
            if (_subscriptions.TryGetValue(messageType.FullName, out var subscriptions))
                return subscriptions;
            return null;
        }
        public void Register(List<SubscribeInfo> subscriptions)
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
                    _subscriptions[messageKey] = new List<SubscribeInfo>();
                }
                _subscriptions[messageKey].Add(subscription);
            }
        }
    }
}
