namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    /// <summary>
    /// Represents a request interface
    /// </summary>
    /// <typeparam name="TRpcResponse">The type of the response</typeparam>
    public interface IRpcRequest<TRpcResponse>
        where TRpcResponse : class
    {
    }
}