using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    /// <summary>
    /// Represents an RPC message sender interface
    /// </summary>
    public interface IRpcMessageSender
    {
        /// <summary>
        /// Sends a request
        /// </summary>
        /// <typeparam name="TRpcResponse">The type of the response</typeparam>
        /// <param name="requestData">The request data</param>
        /// <param name="headers">The message header data</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>The result if the processing is successful, otherwise null</returns>
        Task<TRpcResponse?> SendAsync<TRpcResponse>(IRpcRequest<TRpcResponse> requestData, Dictionary<string, object>? headers = null, CancellationToken cancellationToken = default)
            where TRpcResponse : class;
    }
}