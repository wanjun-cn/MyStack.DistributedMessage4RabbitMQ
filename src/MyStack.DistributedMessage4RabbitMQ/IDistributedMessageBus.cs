namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    /// <summary>
    /// Represents an interface for a distributed message bus.
    /// </summary>
    public interface IDistributedMessageBus : IDistributedEventPublisher, IRpcMessageSender
    {
    }
}
