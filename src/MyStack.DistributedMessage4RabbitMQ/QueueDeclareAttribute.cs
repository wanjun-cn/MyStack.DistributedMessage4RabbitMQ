using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    /// <summary>
    /// Represents the attribute for defining a queue.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class QueueDeclareAttribute : Attribute
    {
        /// <summary>
        /// The name of the queue.
        /// </summary>
        public string Name { get; private set; } = default!;
        /// <summary>
        /// Whether the queue is durable.
        /// </summary>
        public bool? Durable { get; set; }
        /// <summary>
        /// Whether the queue is exclusive.
        /// </summary>
        public bool? Exclusive { get; set; }
        /// <summary>
        /// Whether the queue is automatically deleted.
        /// </summary>
        public bool? AutoDelete { get; set; }
        /// <summary>
        /// Additional arguments for the queue.
        /// </summary>
        public Dictionary<string, object?>? Arguments { get; set; }
        public QueueDeclareAttribute(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            Name = name;
        }
    }
}
