namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    /// <summary>
    /// Represents a wrapper for distributed events.
    /// </summary>
    /// <typeparam name="TEventData">Indicates the type of event data.</typeparam>
    public class DistributedEventWrapper<TEventData> : DistributedEventBase
    {
        public TEventData EventData { get; }
        public DistributedEventWrapper(TEventData eventData)
        {
            EventData = eventData;
        }
    }

    /// <summary>
    /// Represents the default wrapper for distributed events.
    /// </summary>
    public class DistributedEventWrapper : DistributedEventWrapper<object>
    {
        public DistributedEventWrapper(object eventData) : base(eventData)
        {
        }
    }
}
