using System.Collections.Generic;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    public class ExchangeDeclareValue
    {
        /// <summary>
        /// Gets or sets the exchange type
        /// </summary>
        public string? ExchangeType { get; set; }
        /// <summary>
        /// Gets or sets the exchange name
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// Gets or sets whether the exchange is durable
        /// </summary>
        public bool Durable { get; set; }
        /// <summary>
        /// Gets or sets whether the exchange is auto-deleted
        /// </summary>
        public bool AutoDelete { get; set; }
        /// <summary>
        /// Gets or sets additional arguments for the exchange
        /// </summary>
        public Dictionary<string, object?>? Arguments { get; set; }
    }
}
