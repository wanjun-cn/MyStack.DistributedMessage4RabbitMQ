using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    /// <summary>
    /// Represents a request handling interface
    /// </summary>
    /// <typeparam name="TRpcRequest">The type of the request</typeparam>
    /// <typeparam name="TRpcResponse">The type of the response</typeparam>
    public interface IRpcRequestHandler<TRpcRequest, TRpcResponse>
        where TRpcRequest : class, IRpcRequest<TRpcResponse>
        where TRpcResponse : class
    {
        /// <summary>
        /// The task of handle a RPC message
        /// </summary>
        /// <param name="requestData">The request object</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>The result after processing</returns>
        Task<TRpcResponse> HandleAsync(TRpcRequest requestData, CancellationToken cancellationToken = default);
    }
}
