namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    /// <summary>
    /// Represents the RabbitMQ configuration options.
    /// </summary>
    public class RabbitMQOptions
    {
        private string? _userName; 
        /// <summary>
        /// Gets or sets the username
        /// </summary>
        public string UserName
        {
            get
            {
                if (string.IsNullOrEmpty(_userName))
                    _userName = "guest";
                return _userName;
            }
            set
            {
                _userName = value;
            }
        }
        private string? _password;
        /// <summary>
        /// Gets or sets the password
        /// </summary>
        public string Password
        {
            get
            {
                if (string.IsNullOrEmpty(_password))
                    _password = "guest";
                return _password;
            }
            set
            {
                _password = value;
            }
        }

        private string? _hostName;
        /// <summary>
        /// Gets or sets the host name
        /// </summary>
        public string HostName
        {
            get
            {
                if (string.IsNullOrEmpty(_hostName))
                    _hostName = "localhost";
                return _hostName;
            }
            set
            {
                _hostName = value;
            }
        }
        private int? _port;
        /// <summary>
        /// Gets or sets the port
        /// </summary>
        public int Port
        {
            get
            {
                _port ??= 5672;
                return _port.Value;
            }
            set
            {
                _port = value;
            }
        }

        private string? _virtualHost;
        /// <summary>
        /// Gets or sets the virtual host name
        /// </summary>
        public string VirtualHost
        {
            get
            {
                if (string.IsNullOrEmpty(_virtualHost))
                    _virtualHost = "/";
                return _virtualHost;
            }
            set
            {
                _virtualHost = value;
            }
        }
        /// <summary>
        /// Gets or sets the routing key prefix
        /// </summary>
        public string? RoutingKeyPrefix { get; set; }

        private int? _rpcTimeout;
        /// <summary>
        /// Gets or sets the RPC timeout (in milliseconds)
        /// </summary>
        public int RPCTimeout
        {
            get
            {
                _rpcTimeout ??= 10000;
                return _rpcTimeout.Value;
            }
            set
            {
                _rpcTimeout = value;
            }
        }
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
