using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{

    /// <summary>
    /// 表示一个请求处理接口
    /// </summary>
    /// <typeparam name="TRpcRequest">请求的类型</typeparam>
    /// <typeparam name="TRpcResponse">返回结果的类型</typeparam>
    public interface IRpcRequestHandler<TRpcRequest, TRpcResponse>
        where TRpcRequest : class, IRpcRequest<TRpcResponse>
        where TRpcResponse : class
    {
        /// <summary>
        /// 处理RPC请求事件
        /// </summary>
        /// <param name="requestData">请求对象</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>返回处理后的结果</returns>
        Task<TRpcResponse> HandleAsync(TRpcRequest requestData, CancellationToken cancellationToken = default);
    }
}
