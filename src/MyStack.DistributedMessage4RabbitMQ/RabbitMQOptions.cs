namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    /// <summary>
    /// Represents the RabbitMQ configuration options.
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
        public int Port { get; set; } = 5672;
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
        public ExchangeDeclareValue ExchangeOptions { get; set; } = new ExchangeDeclareValue();
        /// <summary>
        /// Gets the queue configuration options
        /// </summary>
        public QueueDeclareValue QueueOptions { get; set; } = new QueueDeclareValue();
    }

}
