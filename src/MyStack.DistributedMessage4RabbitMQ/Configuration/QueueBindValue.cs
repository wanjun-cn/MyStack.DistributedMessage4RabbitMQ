using System.Collections.Generic;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ.Configuration
{
    /// <summary>
    /// Represents a queue binding value.
    /// </summary>
    public class QueueBindValue
    {
        /// <summary>
        /// Initializes an instance of the queue binding value.
        /// </summary> 
        public QueueBindValue()
        {
        }

        /// <summary>
        /// The name of the queue.
        /// </summary>
        public string QueueName { get; set; } = default!;
        /// <summary>
        /// The name of the exchange.
        /// </summary>
        public string ExchangeName { get; set; } = default!;
        /// <summary>
        /// The routing key.
        /// </summary>
        public string RoutingKey { get; set; } = default!;
        /// <summary>
        /// Additional arguments for the exchange.
        /// </summary>
        public Dictionary<string, object?>? Arguments { get; set; }
    }
}
