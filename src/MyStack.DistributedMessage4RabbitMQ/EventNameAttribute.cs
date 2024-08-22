using System;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    /// <summary>
    /// 表示自定义事件名称的特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class EventNameAttribute : Attribute
    {
        /// <summary>
        /// 获取事件的名称
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// 初始化一个自定义事件名称的特性类
        /// </summary>
        /// <param name="name"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public EventNameAttribute(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name", "事件名称不能设置为空。");
            Name = name;
        }
    }
}
