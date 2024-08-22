using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Reflection;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    public class DefaultRoutingKeyResolver : IRoutingKeyResolver
    {
        private string? _keyPrefix;
        public DefaultRoutingKeyResolver(IServiceProvider serviceProvider)
        {
            var options = serviceProvider.GetRequiredService<IOptions<RabbitMQOptions>>();
            _keyPrefix = options.Value.RoutingKeyPrefix;
        }
        public virtual string GetRoutingKey(Type eventType)
        {
            var eventNameAttribute = eventType.GetCustomAttribute<EventNameAttribute>();
            if (eventNameAttribute != null)
            {
                return $"{_keyPrefix}{eventNameAttribute.Name}";
            }
            return $"{_keyPrefix}{eventType.FullName}";
        }
    }
}
