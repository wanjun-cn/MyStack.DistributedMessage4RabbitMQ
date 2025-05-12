namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    /// <summary>
    /// Represents an interface for RPC requests.
    /// </summary>
    /// <typeparam name="TRpcResponse">Indicates the type of the RPC response.</typeparam>
    public interface IRpcRequest<TRpcResponse>:IHasMessageMetadata 
        where TRpcResponse : class
    {
    }
}
