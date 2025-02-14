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
        /// Sends a request message
        /// </summary>
        /// <typeparam name="TRpcResponse">The type of the response</typeparam>
        /// <param name="requestData">The request data</param>
        /// <param name="metadata">The metadata of message. 
        /// <para>If the key name starts with  <see cref="MyStackConsts.RABBITMQ_HEADER"/> ("rabbitmq."), it will be set to the header of RabbitMQ message</para>
        /// </param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>The result if the processing is successful, otherwise null</returns>
        Task<TRpcResponse?> SendAsync<TRpcResponse>(IRpcRequest<TRpcResponse> requestData, MessageMetadata? metadata = null, CancellationToken cancellationToken = default)
            where TRpcResponse : class;
    }
}
