using System;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    /// <summary>
    /// 表示自定义消息名称的特性，用于设置消息的名称
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class MessageNameAttribute : Attribute
    {
        /// <summary>
        /// 获取消息的名称
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// 初始化一个自定义消息名称的特性
        /// </summary>
        /// <param name="name"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public MessageNameAttribute(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name", "事件名称不能设置为空。");
            Name = name;
        }
    }
}
