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
            var subscriptions = new List<SubscribeInfo>();
            foreach (var item in _subscriptions)
            {
                if (IsMatch(messageKey, item.Key))
                    subscriptions.AddRange(item.Value);
            }
            return subscriptions;
        }
        public bool IsMatch(string pattern, string input)
        {
            // 将RabbitMQ的通配符转换为正则表达式
            pattern = pattern.Replace(".", @"\.")// 将模式中的.转换为正则表达式中的\.，以匹配实际的点字符
                .Replace("*", "[^.]")// 将模式中*转换为正则表达式中的[^.]，表示匹配任意一个非点字符
                .Replace("#", ".*");//将模式中的#转换为正则表达式中的.*，表示匹配任意数量的任意字符
            return System.Text.RegularExpressions.Regex.IsMatch(input, $"^{pattern}$");
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
