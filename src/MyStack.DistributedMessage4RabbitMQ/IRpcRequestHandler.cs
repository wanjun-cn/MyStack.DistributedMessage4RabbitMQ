using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    /// <summary>
    /// Represents an interface for RPC message handling services.
    /// </summary>
    /// <typeparam name="TRpcRequest">Indicates the type of the RPC request data.</typeparam>
    /// <typeparam name="TRpcResponse">Indicates the type of the RPC response data.</typeparam>
    public interface IRpcRequestHandler<TRpcRequest, TRpcResponse>
        where TRpcRequest : class, IRpcRequest<TRpcResponse>
        where TRpcResponse : class
    {
        /// <summary>
        /// The task for handling the RPC message.
        /// </summary>
        /// <param name="requestData">The RPC request message object.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<TRpcResponse> HandleAsync(TRpcRequest requestData, CancellationToken cancellationToken = default);
    }
}
