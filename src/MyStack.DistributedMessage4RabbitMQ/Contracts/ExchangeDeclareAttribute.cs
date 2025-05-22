using System;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ.Contracts
{
    /// <summary>
    /// Represents an attribute for declaring an exchange.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ExchangeDeclareAttribute : Attribute
    {
        /// <summary>
        /// The name of the exchange.
        /// </summary>
        public string Name { get; private set; } = default!;
        /// <summary>
        /// The type of the exchange.
        /// </summary>
        public string? ExchangeType { get; set; }
        /// <summary>
        /// Whether the exchange is durable. Non-durable exchanges will be cleared when RabbitMQ is closed.
        /// </summary>
        public bool? Durable { get; set; }
        /// <summary>
        /// Whether the exchange is auto-deleted. If there are no queues bound to it, it will be deleted automatically.
        /// </summary>
        public bool? AutoDelete { get; set; }
        /// <summary>
        /// Initializes an instance of the exchange declaration attribute.
        /// </summary>
        /// <param name="name">The name of the exchange.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public ExchangeDeclareAttribute(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            Name = name;
        }
    }
}