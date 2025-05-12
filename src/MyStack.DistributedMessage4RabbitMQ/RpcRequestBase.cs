namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    /// <summary>
    /// Represents an abstract class for RPC requests.
    /// </summary>
    /// <typeparam name="TRpcResponse">Indicates the RPC response type.</typeparam>
    public abstract class RpcRequestBase<TRpcResponse> : IRpcRequest<TRpcResponse>
        where TRpcResponse : class
    {
        /// <summary>
        /// The message metadata.
        /// </summary>
        public MessageMetadata Metadata { get; }
        /// <summary>
        /// Initializes an RPC request object.
        /// </summary>
        protected RpcRequestBase()
        {
            Metadata = new MessageMetadata();
        }
    }
}
