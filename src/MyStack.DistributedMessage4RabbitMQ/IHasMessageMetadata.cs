namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    public interface IHasMessageMetadata
    {
        /// <summary>
        /// The metadata of the message.
        /// </summary>
        MessageMetadata Metadata { get; }
    }
}
