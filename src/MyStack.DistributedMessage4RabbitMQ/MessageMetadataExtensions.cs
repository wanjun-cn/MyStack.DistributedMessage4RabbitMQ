namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    /// <summary>
    /// Represents extensions for message metadata.
    /// </summary>
    public static class MessageMetadataExtensions
    {
        /// <summary>
        /// Adds RabbitMQ request headers.
        /// </summary>
        /// <param name="meta">The message metadata object.</param>
        /// <param name="name">The name of the request header.</param>
        /// <param name="value">The value of the request header.</param>
        /// <returns></returns>
        public static MessageMetadata AddRabbitHeaders(this MessageMetadata meta, string name, string value)
        {
            meta.TryAdd($"{MyStackConsts.RABBITMQ_HEADER}{name}", value);
            return meta;
        }
    }
}
