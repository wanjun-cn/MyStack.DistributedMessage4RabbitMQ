using System;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    /// <summary>
    /// 表示绑定队列特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class QueueBindAttribute : Attribute
    {
        /// <summary>
        /// 队列名称
        /// </summary>
        public string? QueueName { get; set; }
        /// <summary>
        /// 交换机名称
        /// </summary>
        public string? ExchangeName { get; set; }
        /// <summary>
        /// 路由键
        /// </summary>
        public string RoutingKey { get; private set; } = default!;
        /// <summary>
        /// 初始化一个绑定队列
        /// </summary>
        /// <param name="routingKey">路由键</param>
        public QueueBindAttribute(string routingKey)
        {
            RoutingKey = routingKey;
        }
    }
}
