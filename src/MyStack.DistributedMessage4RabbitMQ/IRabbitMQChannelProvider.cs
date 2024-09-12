using RabbitMQ.Client;
using System;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    /// <summary>
    /// 表示RabbitMQ消息通道提供器
    /// </summary>
    public interface IRabbitMQChannelProvider : IDisposable
    {
        /// <summary>
        /// 创建一个通道对象
        /// </summary>
        /// <returns></returns>
        IModel CreateModel();
    }
}
