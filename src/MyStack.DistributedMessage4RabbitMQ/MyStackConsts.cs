namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    /// <summary>
    /// Represents constants.
    /// </summary>
    public static class MyStackConsts
    {
        /// <summary>
        /// The prefix for RabbitMQ message headers.
        /// </summary>
        public const string MESSAGE_HEADER = "rabbitmq.";
        /// <summary>
        /// The default RabbitMQ exchange name.
        /// </summary>
        public const string DEFAULT_EXCHANGE_NAME = "MyStack";
        /// <summary>
        /// The default RabbitMQ queue name.
        /// </summary>
        public const string DEFAULT_QUEUE_NAME = "MyStack";
    }
}
