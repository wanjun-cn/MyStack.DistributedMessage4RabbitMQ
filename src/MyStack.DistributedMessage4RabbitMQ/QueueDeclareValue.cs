using System.Collections.Generic;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    public class QueueDeclareValue
    {
        /// <summary>
        /// Gets or sets the queue name
        /// </summary>
        public string Name { get; set; } = default!;
        /// <summary>
        /// Gets or sets whether the queue is durable
        /// </summary>
        public bool Durable { get; set; }
        /// <summary>
        /// Gets or sets whether the queue is exclusive
        /// </summary>
        public bool Exclusive { get; set; }
        /// <summary>
        /// Gets or sets whether the queue is auto-deleted
        /// </summary>
        public bool AutoDelete { get; set; }
        /// <summary>
        /// Gets or sets additional arguments for the queue
        /// </summary>
        public Dictionary<string, object?>? Arguments { get; set; }
    }
}
