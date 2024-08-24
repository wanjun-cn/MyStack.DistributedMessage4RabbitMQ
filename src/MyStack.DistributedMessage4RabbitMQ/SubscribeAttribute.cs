using System;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    /// <summary>
    /// 表示一个消息订阅特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SubscribeAttribute : Attribute
    {
        /// <summary>
        /// 获取订阅的键名
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 初始化一个消息订阅对象
        /// </summary>
        /// <param name="key">订阅键名</param>
        /// <exception cref="ArgumentNullException"></exception>
        public SubscribeAttribute(string key)
        {
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key),"订阅键不能为空");
            Key = key;
        }
    }
}
