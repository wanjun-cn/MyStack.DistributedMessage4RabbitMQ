using System;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    /// <summary>
    /// 表示一个消息元数据特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class MessageMetadataAttribute : Attribute
    {
        /// <summary>
        /// 获取元数据的名称
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// 获取元数据的值
        /// </summary>
        public object Value { get; }
        /// <summary>
        /// 初始化一个消息元数据特性
        /// </summary>
        /// <param name="name">元数据的名称</param>
        /// <param name="value">元数据的值</param>
        /// <exception cref="ArgumentNullException"></exception>
        public MessageMetadataAttribute(string name, object value)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name", "元数据的名称不能设置为空。");
            Name = name;
            if (value == null) throw new ArgumentNullException("value", "元数据的值不能设置为空。");
            Value = value;
        }
    }
}
