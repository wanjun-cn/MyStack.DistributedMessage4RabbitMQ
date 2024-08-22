using System.Collections.Generic;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    /// <summary>
    /// Represents a RabbitMQ configuration
    /// </summary>
    public class RabbitMQOptions
    {
        /// <summary>
        /// Get or set UserName
        /// </summary>
        public string UserName { get; set; } = "guest";
        /// <summary>
        /// Get or set Password
        /// </summary>
        public string Password { get; set; } = "guest";
        /// <summary>
        /// Get or set Host name
        /// </summary>
        public string HostName { get; set; } = "localhost";
        /// <summary>
        /// Get or set Port
        /// </summary>
        public int Port { get; set; } = 3306;
        /// <summary>
        /// Get or set Virtual host
        /// </summary>
        public string VirtualHost { get; set; } = "/";
        /// <summary>
        /// Get or set Prefix for routing keys
        /// </summary>
        public string? RoutingKeyPrefix { get; set; }
        /// <summary>
        /// Get or set the waiting time for RPC (millisecond)
        /// </summary>
        public int RPCTimeout { get; set; } = 10000;
        public RabbitMQExchangeOptions ExchangeOptions { get; set; } = new RabbitMQExchangeOptions();
        public RabbitMQQueueOptions QueueOptions { get; set; } = new RabbitMQQueueOptions();
    }
    /// <summary>
    /// Represents a RabbitMQ exchange configuration
    /// </summary>
    public class RabbitMQExchangeOptions
    {
        /// <summary>
        /// Get or set Exchange type
        /// </summary>
        public string ExchangeType { get; set; } = default!;
        /// <summary>
        /// Get or set Exchange name
        /// </summary>
        public string Name { get; set; } = default!;
        /// <summary>
        /// Get or set Exchange can be durable or transient
        /// </summary>
        public bool Durable { get; set; }
        public bool AutoDelete { get; set; }
        public Dictionary<string, object>? Arguments { get; set; }
    }
    public class RabbitMQQueueOptions
    {
        public string Name { get; set; } = default!;
        public bool Durable { get; set; }
        public bool Exclusive { get; set; }
        public bool AutoDelete { get; set; }
        public Dictionary<string, object>? Arguments { get; set; }
    }
}
