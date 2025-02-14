namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    /// <summary>
    /// Represents a distributed event
    /// </summary>
    public class DistributedEventBase : IDistributedEvent
    {
        public MessageMetadata Metadata { get; }
        public DistributedEventBase()
        {
            Metadata = new MessageMetadata();
        }
    }
}
