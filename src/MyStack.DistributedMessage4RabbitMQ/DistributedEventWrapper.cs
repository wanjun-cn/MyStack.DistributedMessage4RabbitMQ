namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    /// <summary>
    /// A generic wrapper for distributed events, containing event data of type <typeparamref name="TEventData"/>.
    /// </summary>
    /// <typeparam name="TEventData">The type of the event data.</typeparam>
    public class DistributedEventWrapper<TEventData> : DistributedEventBase
    {
        /// <summary>
        /// Gets the event data associated with the distributed event.
        /// </summary>
        public TEventData EventData { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DistributedEventWrapper{TEventData}"/> class with the specified event data.
        /// </summary>
        /// <param name="eventData">The event data to wrap.</param>
        public DistributedEventWrapper(TEventData eventData)
        {
            EventData = eventData;
        }
    }

    /// <summary>
    /// A non-generic wrapper for distributed events, containing event data as an object.
    /// </summary>
    public class DistributedEventWrapper : DistributedEventWrapper<object>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DistributedEventWrapper"/> class with the specified event data.
        /// </summary>
        /// <param name="eventData">The event data to wrap.</param>
        public DistributedEventWrapper(object eventData) : base(eventData)
        {
        }
    }
}
