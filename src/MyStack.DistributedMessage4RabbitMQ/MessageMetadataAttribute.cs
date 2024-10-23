using System;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    /// <summary>
    /// Represents a message metadata attribute, used to mark metadata for message passing
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class MessageMetadataAttribute : Attribute
    {
        /// <summary>
        /// Gets the name of the metadata
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Gets the value of the metadata
        /// </summary>
        public object Value { get; }
        /// <summary>
        /// Initializes a message metadata attribute
        /// </summary>
        /// <param name="name">The name of the metadata</param>
        /// <param name="value">The value of the metadata</param>
        /// <exception cref="ArgumentNullException"></exception>
        public MessageMetadataAttribute(string name, object value)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name", "The name of the metadata cannot be set to null or empty.");
            Name = name;
            Value = value ?? throw new ArgumentNullException("value", "The value of the metadata cannot be set to null.");
        }
    }
}