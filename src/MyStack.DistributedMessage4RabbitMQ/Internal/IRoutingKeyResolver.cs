using System;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ.Internal
{
    /// <summary>
    /// Represents a routing key resolver
    /// </summary>
    internal interface IRoutingKeyResolver
    {
        /// <summary>
        /// Gets the routing key for the message type
        /// </summary>
        /// <param name="messageType"></param>
        /// <returns></returns>
        string GetRoutingKey(Type messageType);

        /// <summary>
        /// Gets the routing key for the custom key
        /// </summary>
        /// <param name="customKey"></param>
        /// <returns></returns>
        string GetRoutingKey(string customKey);
    }
}