namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    /// <summary>
    /// Represents extensions for message metadata.
    /// </summary>
    public static class MessageMetadataExtensions
    {
        /// <summary>
        /// Adds RabbitMQ message headers.
        /// </summary>
        /// <param name="metadata">The message metadata object.</param>
        /// <param name="name">The name of the header.</param>
        /// <param name="value">The value of the header.</param>
        /// <returns></returns>
        public static MessageMetadata AddMessageHeader(this MessageMetadata metadata, string name, string value)
        {
            metadata.TryAdd($"{MyStackConsts.MESSAGE_HEADER}{name}", value);
            return metadata;
        }
    }
}
