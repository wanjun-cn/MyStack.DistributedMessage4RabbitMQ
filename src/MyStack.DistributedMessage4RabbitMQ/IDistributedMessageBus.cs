using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    /// <summary>
    /// 表示分布式消息总线接口
    /// </summary>
    public interface IDistributedMessageBus
    {
        /// <summary>
        /// 发布一个事件
        /// </summary>
        /// <param name="eventData">事件数据</param>
        /// <param name="metadata">事件元数据</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        Task PublishAsync(IDistributedEvent eventData, Dictionary<string, object>? metadata = null, CancellationToken cancellationToken = default);
        /// <summary>
        /// 发送一个事件体
        /// </summary>
        /// <param name="eventData">事件数据</param>
        /// <param name="metadata">事件元数据</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        Task PublishAsync(object eventData, Dictionary<string, object>? metadata = null, CancellationToken cancellationToken = default);
        /// <summary>
        /// 发送一个请求
        /// </summary>
        /// <typeparam name="TRpcResponse">返回结果的类型</typeparam>
        /// <param name="requestData">请求数据</param>
        /// <param name="metadata">请求元数据</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>处理成功时返回结果，否则返回null</returns>
        Task<TRpcResponse?> SendAsync<TRpcResponse>(IRpcRequest<TRpcResponse> requestData, Dictionary<string, object>? metadata = null, CancellationToken cancellationToken = default)
            where TRpcResponse : class;
    }
}
