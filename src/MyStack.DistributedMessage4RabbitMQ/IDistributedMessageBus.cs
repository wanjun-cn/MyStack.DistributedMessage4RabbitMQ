namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    /// <summary>
    /// Represents a distributed message bus interface
    /// </summary>
    public interface IDistributedMessageBus : IDistributedEventPublisher, IRpcMessageSender
    {
    }
}