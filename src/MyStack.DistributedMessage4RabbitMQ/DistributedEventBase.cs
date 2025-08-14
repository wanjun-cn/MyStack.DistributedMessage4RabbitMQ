namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    /// <summary>  
    /// Represents the base class for distributed events.  
    /// </summary>  
    public abstract class DistributedEventBase : IDistributedEvent
    {
        /// <summary>  
        /// Gets the metadata associated with the message.  
        /// </summary>  
        public MessageMetadata Metadata { get; }

        /// <summary>  
        /// Initializes a new instance of the <see cref="DistributedEventBase"/> class.  
        /// </summary>  
        public DistributedEventBase()
        {
            Metadata = new MessageMetadata();
        }
    }
}
