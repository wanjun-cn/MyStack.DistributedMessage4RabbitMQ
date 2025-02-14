using System;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    public class RpcTimeoutException : Exception
    {
        public RpcTimeoutException(string message, int timeout, string routingKey) : base(message)
        {
            Timeout = timeout;
            RoutingKey = routingKey;
        }
        public int Timeout { get; private set; }
        public string RoutingKey { get; private set; }
    }
}
