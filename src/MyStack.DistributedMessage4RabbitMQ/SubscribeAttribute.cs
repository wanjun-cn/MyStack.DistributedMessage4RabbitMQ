using System;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    /// <summary>
    /// Represents a message subscription attribute, used to help event subscription handlers describe subscription information
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SubscribeAttribute : Attribute
    {
        /// <summary>
        /// Gets the subscription key
        /// </summary>
        public string Key { get; }
        /// <summary>
        /// Initializes a message subscription attribute
        /// </summary>
        /// <param name="key">Subscription key</param>
        /// <exception cref="ArgumentNullException"></exception>
        public SubscribeAttribute(string key)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key), "Subscription key cannot be null or empty");
            Key = key;
        }
    }
}