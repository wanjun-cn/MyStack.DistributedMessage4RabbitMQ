namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    /// <summary>
    /// 表示一个请求接口
    /// </summary>
    /// <typeparam name="TRpcResponse"></typeparam>
    public interface IRpcRequest<TRpcResponse>
         where TRpcResponse : class
    {
    }
}
