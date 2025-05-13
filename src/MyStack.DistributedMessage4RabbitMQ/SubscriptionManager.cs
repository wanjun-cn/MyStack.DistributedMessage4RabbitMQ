using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    internal class SubscriptionManager : ISubscriptionRegistrar
    {
        private static readonly Dictionary<Type, List<string>> _subscriptions = new Dictionary<Type, List<string>>();
        private readonly QueueBindValueProvider _queueBindValueProvider;
        public SubscriptionManager(QueueBindValueProvider queueBindValueProvider)
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

        public IList<Type>? GetSubscriptions(string messageKey)
        {
            var subscriptions = new List<Type>();
            foreach (var subscription in _subscriptions)
            {
                foreach (var subscribeKey in subscription.Value)
                {
                    if (IsMatch(subscribeKey, messageKey))
                    {
                        subscriptions.Add(subscription.Key);
                        break;
                    }
                }
            }
            return subscriptions;
        }
        public IList<Type>? GetAllSubscriptions()
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

    }
}
