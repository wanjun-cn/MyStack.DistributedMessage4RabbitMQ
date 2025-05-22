using System;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ.Contracts
{
    /// <summary>  
    /// Represents the queue binding attribute.  
    /// </summary>  
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class QueueBindAttribute : Attribute
    {
        /// <summary>  
        /// The name of the queue.  
        /// </summary>  
        public string? QueueName { get; set; }
        /// <summary>  
        /// The name of the exchange.  
        /// </summary>  
        public string? ExchangeName { get; set; }
        /// <summary>  
        /// The routing key.  
        /// </summary>  
        public string RoutingKey { get; private set; } = default!;

        /// <summary>  
        /// Initializes a queue binding.  
        /// </summary>  
        /// <param name="routingKey">The routing key.</param>  
        public QueueBindAttribute(string routingKey)
        {
            RoutingKey = routingKey;
        }
    }
}
