using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    /// <summary>
    /// 表示分布式事件发布接口
    /// </summary>
    public interface IDistributedEventPublisher
    {
        /// <summary>
        /// 发布一个事件
        /// </summary>
        /// <param name="key">事件的键名</param>
        /// <param name="eventData">事件数据</param>
        /// <param name="metadata">事件元数据</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        Task PublishAsync(string key, object eventData, Dictionary<string, object>? metadata = null, CancellationToken cancellationToken = default);
        /// <summary>
        /// 发布一个事件
        /// </summary>
        /// <param name="eventData">事件数据</param>
        /// <param name="metadata">事件元数据</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        Task PublishAsync(IDistributedEvent eventData, Dictionary<string, object>? metadata = null, CancellationToken cancellationToken = default);
        /// <summary>
        /// 发布一个事件对象
        /// </summary>
        /// <param name="eventData">事件数据</param>
        /// <param name="metadata">事件元数据</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        Task PublishAsync(object eventData, Dictionary<string, object>? metadata = null, CancellationToken cancellationToken = default);
    }
}
