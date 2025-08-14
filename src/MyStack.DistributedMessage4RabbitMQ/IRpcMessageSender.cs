using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    /// <summary>
    /// Represents an interface for RPC message sending services.
    /// </summary>
    public interface IRpcMessageSender
    {
        /// <summary>
        /// Sends an RPC message.
        /// </summary>
        /// <typeparam name="TRpcResponse">Indicates the type of the RPC response message.</typeparam>
        /// <param name="requestData">The request message object.</param>
        /// <param name="metadata">The metadata of the message.
        /// <para>If the key of the metadata is prefixed with <see cref="MyStackConsts.MESSAGE_HEADER"/> ("rabbitmq."), the message header will be set in the RabbitMQ request header.</para>
        /// </param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<TRpcResponse?> SendAsync<TRpcResponse>(IRpcRequest<TRpcResponse> requestData, MessageMetadata? metadata = null, CancellationToken cancellationToken = default)
            where TRpcResponse : class;
    }
}
