using System;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class QueueBindAttribute : Attribute
    {
        public string? QueueName { get; set; }
        public string? ExchangeName { get; set; }
        public string RoutingKey { get; private set; } = default!;
        public QueueBindAttribute(string routingKey)
        {
            RoutingKey = routingKey;
        }
    }
}
