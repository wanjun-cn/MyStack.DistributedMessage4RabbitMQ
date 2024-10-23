using System.Collections.Generic;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    /// <summary>
    /// Represents a RabbitMQ configuration option
    /// </summary>
    public class RabbitMQOptions
    {
        /// <summary>
        /// Gets or sets the username
        /// </summary>
        public string UserName { get; set; } = "guest";
        /// <summary>
        /// Gets or sets the password
        /// </summary>
        public string Password { get; set; } = "guest";
        /// <summary>
        /// Gets or sets the host name
        /// </summary>
        public string HostName { get; set; } = "localhost";
        /// <summary>
        /// Gets or sets the port
        /// </summary>
        public int Port { get; set; } = 5672; // Corrected the default port for RabbitMQ
        /// <summary>
        /// Gets or sets the virtual host name
        /// </summary>
        public string VirtualHost { get; set; } = "/";
        /// <summary>
        /// Gets or sets the routing key prefix
        /// </summary>
        public string? RoutingKeyPrefix { get; set; }
        /// <summary>
        /// Gets or sets the RPC timeout (in milliseconds)
        /// </summary>
        public int RPCTimeout { get; set; } = 10000;
        /// <summary>
        /// Gets the exchange configuration options
        /// </summary>
        public RabbitMQExchangeOptions ExchangeOptions { get; set; } = new RabbitMQExchangeOptions();
        /// <summary>
        /// Gets the queue configuration options
        /// </summary>
        public RabbitMQQueueOptions QueueOptions { get; set; } = new RabbitMQQueueOptions();
    }

    /// <summary>
    /// Represents exchange configuration options
    /// </summary>
    public class RabbitMQExchangeOptions
    {
        /// <summary>
        /// Gets or sets the exchange type
        /// </summary>
        public string ExchangeType { get; set; } = default!;
        /// <summary>
        /// Gets or sets the exchange name
        /// </summary>
        public string Name { get; set; } = default!;
        /// <summary>
        /// Gets or sets whether the exchange is durable
        /// </summary>
        public bool Durable { get; set; }
        /// <summary>
        /// Gets or sets whether the exchange is auto-deleted
        /// </summary>
        public bool AutoDelete { get; set; }
        /// <summary>
        /// Gets or sets additional arguments for the exchange
        /// </summary>
        public Dictionary<string, object>? Arguments { get; set; }
    }

    /// <summary>
    /// Represents queue configuration options
    /// </summary>
    public class RabbitMQQueueOptions
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
        public Dictionary<string, object>? Arguments { get; set; }
    }
}