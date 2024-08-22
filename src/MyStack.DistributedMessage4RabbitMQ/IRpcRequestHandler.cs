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
        Task<TRpcResponse> HandleAsync(TRpcRequest eventData, CancellationToken cancellationToken = default);
    }
}
