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
        /// <summary>
        /// 处理事件
        /// </summary>
        /// <param name="eventData">事件对象</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        Task HandleAsync(TEvent eventData, CancellationToken cancellationToken = default);
    }
    /// <summary>
    /// 表示一个分布式事件订阅接口
    /// </summary>
    public interface IDistributedEventHandler
    {
        /// <summary>
        /// 处理事件
        /// </summary>
        /// <param name="eventData">事件对象</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        Task HandleAsync(object eventData, CancellationToken cancellationToken = default);
    }
}
