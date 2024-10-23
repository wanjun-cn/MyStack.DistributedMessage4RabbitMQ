using System;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    /// <summary>
    /// Represents a custom message name attribute, used to set the name of the message
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class MessageNameAttribute : Attribute
    {
        /// <summary>
        /// Gets the name of the message
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Initializes a custom message name attribute
        /// </summary>
        /// <param name="name">The name of the message</param>
        /// <exception cref="ArgumentNullException"></exception>
        public MessageNameAttribute(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name", "The message name cannot be set to null or empty.");
            Name = name;
        }
    }
}