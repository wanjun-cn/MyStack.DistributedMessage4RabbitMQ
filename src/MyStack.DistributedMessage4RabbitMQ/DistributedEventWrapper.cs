namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    /// <summary>
    /// Represents an event wrapper
    /// </summary>
    /// <typeparam name="TEventData">The type of event data</typeparam>
    public class DistributedEventWrapper<TEventData> : IDistributedEvent
    {
        public TEventData EventData { get; }
        public DistributedEventWrapper(TEventData eventData)
        {
            EventData = eventData;
        }
    }

    /// <summary>
    /// Represents an event wrapper
    /// </summary>
    public class DistributedEventWrapper : DistributedEventWrapper<object>
    {
        public DistributedEventWrapper(object eventData) : base(eventData)
        {
        }
    }
}