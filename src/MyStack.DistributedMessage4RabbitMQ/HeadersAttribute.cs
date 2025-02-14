using System;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    [AttributeUsage(AttributeTargets.Class)]
    public class HeadersAttribute : Attribute
    {
        public string Key { get; } = default!;
        public string Value { get; } = default!;
        public HeadersAttribute(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}
