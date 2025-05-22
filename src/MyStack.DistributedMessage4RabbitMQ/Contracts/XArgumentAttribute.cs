using System;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ.Contracts
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class XArgumentAttribute : Attribute
    {
        public XArgumentType Type { get; } = default!;
        public string Key { get; }
        public object Value { get; }
        public XArgumentAttribute(XArgumentType type, string key, object value)
        {
            Type = type;
            Key = key;
            Value = value;
        }
    }

}
