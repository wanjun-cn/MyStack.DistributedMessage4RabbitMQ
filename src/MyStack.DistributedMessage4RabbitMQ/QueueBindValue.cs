namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    /// <summary>
    /// Represents a queue binding value.
    /// </summary>
    public class QueueBindValue
    {
        /// <summary>
        /// Initializes an instance of the queue binding value.
        /// </summary>
        /// <param name="queueName">The name of the queue.</param>
        /// <param name="exchangeName">The name of the exchange.</param>
        /// <param name="routingKey">The routing key.</param>
        public QueueBindValue(string queueName, string exchangeName, string routingKey)
        {
            QueueName = queueName;
            ExchangeName = exchangeName;
            RoutingKey = routingKey;
        }

        /// <summary>
        /// The name of the queue.
        /// </summary>
        public string QueueName { get; set; }
        /// <summary>
        /// The name of the exchange.
        /// </summary>
        public string ExchangeName { get; set; }
        /// <summary>
        /// The routing key.
        /// </summary>
        public string RoutingKey { get; set; } = default!;
    }
}
