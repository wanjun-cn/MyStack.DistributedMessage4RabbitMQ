using System.Collections.Generic;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ.Configuration
{
    /// <summary>
    /// Represents the value of an exchange.
    /// </summary>
    public class ExchangeDeclareValue
    {
        /// <summary>
        /// The name of the exchange.
        /// </summary>
        public string Name { get; set; } = default!;
        /// <summary>
        /// The type of the exchange.
        /// </summary>
        public string ExchangeType { get; set; } = default!;
        /// <summary>
        /// Whether the exchange is durable. Non-durable exchanges will be cleared when RabbitMQ is closed.
        /// </summary>
        public bool Durable { get; set; }
        /// <summary>
        /// Whether the exchange is auto-deleted. If there are no queues bound to it, it will be deleted automatically.
        /// </summary>
        public bool AutoDelete { get; set; }
        /// <summary>
        /// Additional arguments for the exchange.
        /// </summary>
        public Dictionary<string, object?>? Arguments { get; set; }
    }
}
