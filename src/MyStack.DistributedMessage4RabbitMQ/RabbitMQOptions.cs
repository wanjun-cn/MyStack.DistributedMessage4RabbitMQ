using System.Collections.Generic;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    /// <summary>
    /// 表示一个RabbitMQ配置项
    /// </summary>
    public class RabbitMQOptions
    {
        /// <summary>
        /// 获取或设置用户名
        /// </summary>
        public string UserName { get; set; } = "guest";
        /// <summary>
        /// 获取或设置密码
        /// </summary>
        public string Password { get; set; } = "guest";
        /// <summary>
        /// 获取或设置主机名称
        /// </summary>
        public string HostName { get; set; } = "localhost";
        /// <summary>
        /// 获取或设置端口
        /// </summary>
        public int Port { get; set; } = 3306;
        /// <summary>
        /// 获取或设置虚拟主机名称
        /// </summary>
        public string VirtualHost { get; set; } = "/";
        /// <summary>
        /// 获取或设置路由键前缀名称
        /// </summary>
        public string? RoutingKeyPrefix { get; set; }
        /// <summary>
        /// 获取或设置RPC超时时间（毫秒）
        /// </summary>
        public int RPCTimeout { get; set; } = 10000;
        /// <summary>
        /// 获取交换机配置项
        /// </summary>
        public RabbitMQExchangeOptions ExchangeOptions { get; set; } = new RabbitMQExchangeOptions();
        /// <summary>
        /// 获取队列配置项
        /// </summary>
        public RabbitMQQueueOptions QueueOptions { get; set; } = new RabbitMQQueueOptions();
    }
    /// <summary>
    /// 表示交换机配置项
    /// </summary>
    public class RabbitMQExchangeOptions
    {
        /// <summary>
        /// 获取或设置交换机类型
        /// </summary>
        public string ExchangeType { get; set; } = default!;
        /// <summary>
        /// 获取或设置交换机名称
        /// </summary>
        public string Name { get; set; } = default!;
        /// <summary>
        /// 获取或设置是否持久化交换机
        /// </summary>
        public bool Durable { get; set; }
        public bool AutoDelete { get; set; }
        public Dictionary<string, object>? Arguments { get; set; }
    }
    /// <summary>
    /// 表示队列配置项
    /// </summary>
    public class RabbitMQQueueOptions
    {
        public string Name { get; set; } = default!;
        public bool Durable { get; set; }
        public bool Exclusive { get; set; }
        public bool AutoDelete { get; set; }
        public Dictionary<string, object>? Arguments { get; set; }
    }
}
