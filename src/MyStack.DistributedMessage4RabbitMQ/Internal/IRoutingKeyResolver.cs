using System;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ.Internal
{
    /// <summary>
    /// 表示路由键解析器
    /// </summary>
    internal interface IRoutingKeyResolver
    {
        /// <summary>
        /// 获取消息类型的路由键
        /// </summary>
        /// <param name="messageType"></param>
        /// <returns></returns>
        string GetRoutingKey(Type messageType);
        /// <summary>
        /// 获取自定义键的路由键
        /// </summary>
        /// <param name="customKey"></param>
        /// <returns></returns>
        string GetRoutingKey(string customKey);
    }
}
