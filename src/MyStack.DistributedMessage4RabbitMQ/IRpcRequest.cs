namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    /// <summary>
    /// 表示一个请求接口
    /// </summary>
    /// <typeparam name="TRpcResponse">表示响应的类型</typeparam>
    public interface IRpcRequest<TRpcResponse>
         where TRpcResponse : class
    {
    }
}
