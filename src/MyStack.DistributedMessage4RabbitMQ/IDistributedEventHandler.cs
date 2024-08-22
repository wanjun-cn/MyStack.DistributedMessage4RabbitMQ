using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{

    /// <summary>
    /// 表示一个分布式事件订阅接口
    /// </summary>
    /// <typeparam name="TEvent">事件的类型</typeparam>
    public interface IDistributedEventHandler<TEvent>
        where TEvent : class, IDistributedEvent
    {
        Task HandleAsync(TEvent eventData, CancellationToken cancellationToken = default);
    }

}
