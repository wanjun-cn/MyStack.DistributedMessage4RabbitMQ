namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    /// <summary>
    /// Represents a request
    /// </summary>
    /// <typeparam name="TRpcResponse">The type of the response</typeparam>
    public abstract class RpcRequestBase<TRpcResponse> : IRpcRequest<TRpcResponse>
        where TRpcResponse : class
    {
        public MessageMetadata Metadata { get; }
        protected RpcRequestBase()
        {
            Metadata = new MessageMetadata();
        }
    }
}
