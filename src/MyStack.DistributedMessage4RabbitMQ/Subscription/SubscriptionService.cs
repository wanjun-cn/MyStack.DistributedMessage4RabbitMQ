using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DistributedMessage4RabbitMQ.Configuration;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ.Subscription
{
    public class SubscriptionService : ISubscriptionService, ISubscriptionRegistrar
    {
        private static readonly Dictionary<Type, List<string>> _subscriptions = new Dictionary<Type, List<string>>();
        private Dictionary<string, List<Type>> RoutingKeyMaps = new Dictionary<string, List<Type>>();
        private readonly QueueBindValueProvider _queueBindValueProvider;
        public SubscriptionService(QueueBindValueProvider queueBindValueProvider)
        {
            _queueBindValueProvider = queueBindValueProvider;
        }
        public void Subscribe(params Type[] messagetypes)
        {
            if (messagetypes == null) throw new ArgumentNullException(nameof(messagetypes));
            foreach (var messageType in messagetypes)
            {
                if (!_subscriptions.TryGetValue(messageType, out List<string>? value))
                {
                    value = new List<string>();
                    _subscriptions[messageType] = value;
                }
                var queueBindValue = _queueBindValueProvider.GetValue(messageType);
                value.Add(queueBindValue.RoutingKey);
            }
        }

        public IReadOnlyList<Type>? GetMessageTypes(string routingKey)
        {
            List<Type>? messageTypes;
            if (RoutingKeyMaps.TryGetValue(routingKey, out messageTypes))
                return messageTypes;
            else
            {
                messageTypes = new List<Type>();
                foreach (var subscription in _subscriptions)
                {
                    foreach (var subscribeKey in subscription.Value)
                    {
                        if (IsMatch(subscribeKey, routingKey))
                        {
                            messageTypes.Add(subscription.Key);
                            break;
                        }
                    }
                }
                if (RoutingKeyMaps.ContainsKey(routingKey))
                    RoutingKeyMaps[routingKey] = messageTypes;
                else
                    RoutingKeyMaps.TryAdd(routingKey, messageTypes);
                return messageTypes;
            }

        }
        public IReadOnlyList<Type>? GetAllMessageTypes()
        {
            return _subscriptions.Keys.ToList();
        }
        public bool IsMatch(string pattern, string input)
        {
            // Convert RabbitMQ wildcards to regular expressions
            pattern = pattern.Replace(".", @"\.")  // Replace . in the pattern with \. to match actual dot characters
                .Replace("*", "[^.]*")  // Replace * in the pattern with [^.]* to match any character except a dot
                .Replace("#", ".*");  // Replace # in the pattern with .* to match any number of any characters
            return System.Text.RegularExpressions.Regex.IsMatch(input, $"^{pattern}$");
        }

        public bool IsEvent(string routingKey)
        {
            var messageTypes = GetMessageTypes(routingKey);
            if (messageTypes == null || messageTypes.Count == 0)
                return false;
            foreach (var messageType in messageTypes)
            {
                if (messageType.GetInterfaces().Any(x => x == typeof(IRpcRequest<>)))
                    throw new InvalidOperationException("Message type implements IRpcRequest<> interface, cannot be used as an event.");
            }
            return messageTypes[0].GetInterfaces().Any(x => x == typeof(IDistributedEvent));
        }

        public bool IsRpcRequest(string routingKey)
        {
            var messageTypes = GetMessageTypes(routingKey);
            if (messageTypes == null || messageTypes.Count == 0)
                return false;
            if (messageTypes.Count > 1)
                throw new InvalidOperationException("Multiple message types found for the same routing key.");
            return messageTypes[0].GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IRpcRequest<>));
        }
    }
}
