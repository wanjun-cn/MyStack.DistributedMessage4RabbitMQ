namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    /// <summary>
    /// 表示分布式消息总线接口
    /// </summary>
    public interface IDistributedMessageBus : IDistributedEventPublisher, IRpcMessageSender
    {
    }
}
