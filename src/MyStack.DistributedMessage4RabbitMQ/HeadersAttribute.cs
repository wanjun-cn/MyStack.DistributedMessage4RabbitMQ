using System;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    /// <summary>
    /// Represents an attribute for message headers.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class HeadersAttribute : Attribute
    {
        /// <summary>
        /// The key of the header.
        /// </summary>
        public string Key { get; } = default!;
        /// <summary>
        /// The value of the header.
        /// </summary>
        public string Value { get; } = default!;
        /// <summary>
        /// Initializes an instance of the message header attribute.
        /// </summary>
        /// <param name="key">The key of the header.</param>
        /// <param name="value">The value of the header.</param>
        public HeadersAttribute(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}
