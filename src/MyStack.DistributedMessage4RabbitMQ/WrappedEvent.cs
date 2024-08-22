namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    public class WrappedEvent<TData> : IDistributedEvent
    {
        public TData Data { get; }
        public WrappedEvent(TData data)
        {
            Data = data;
        }
    }
    public class WrappedEvent : WrappedEvent<object>
    {
        public WrappedEvent(object data) : base(data)
        {
        }
    }
}
