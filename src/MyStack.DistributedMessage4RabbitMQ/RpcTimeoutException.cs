using System;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    /// <summary>
    /// Represents an RPC timeout exception.
    /// </summary>
    public class RpcTimeoutException : Exception
    {
        /// <summary>
        /// Initializes an RPC timeout exception object.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="timeout">The timeout duration.</param>
        /// <param name="routingKey">The routing key.</param>
        public RpcTimeoutException(string message, int timeout, string routingKey) : base(message)
        {
            Timeout = timeout;
            RoutingKey = routingKey;
        }
        public int Timeout { get; private set; }
        public string RoutingKey { get; private set; }
    }
}
