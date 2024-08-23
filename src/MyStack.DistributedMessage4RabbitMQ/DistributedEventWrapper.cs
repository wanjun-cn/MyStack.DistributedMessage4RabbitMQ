namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    /// <summary>
    /// 表示一个事件体
    /// </summary>
    /// <typeparam name="TEventData">事件数据类型</typeparam>
    public class DistributedEventWrapper<TEventData> : IDistributedEvent
    {
        public TEventData EventData { get; }
        public DistributedEventWrapper(TEventData eventData)
        {
            EventData = eventData;
        }
    }
    /// <summary>
    /// 表示一个事件体
    /// </summary>
    public class DistributedEventWrapper : DistributedEventWrapper<object>
    {
        public DistributedEventWrapper(object eventData) : base(eventData)
        {
        }
    }
}
