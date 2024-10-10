using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    /// <summary>
    /// 表示RPC消息发送接口
    /// </summary>
    public interface IRpcMessageSender
    {
        /// <summary>
        /// 发送一个请求
        /// </summary>
        /// <typeparam name="TRpcResponse">返回结果的类型</typeparam>
        /// <param name="requestData">请求数据</param>
        /// <param name="headers">消息头数据</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>处理成功时返回结果，否则返回null</returns>
        Task<TRpcResponse?> SendAsync<TRpcResponse>(IRpcRequest<TRpcResponse> requestData, Dictionary<string, object>? headers = null, CancellationToken cancellationToken = default)
            where TRpcResponse : class;
    }
}
