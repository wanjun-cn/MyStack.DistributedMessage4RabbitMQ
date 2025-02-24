namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    /// <summary>
    /// Represents an abstract class for distributed events.
    /// </summary>
    public abstract class DistributedEventBase : IDistributedEvent
    {
        /// <summary>
        /// Message metadata.
        /// </summary>
        public MessageMetadata Metadata { get; }
        /// <summary>
        /// Initializes a distributed event object.
        /// </summary>
        public DistributedEventBase()
        {
            Metadata = new MessageMetadata();
        }
    }
}
