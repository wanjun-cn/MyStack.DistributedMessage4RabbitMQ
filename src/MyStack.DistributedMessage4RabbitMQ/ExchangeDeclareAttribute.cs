using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ExchangeDeclareAttribute : Attribute
    {
        public string Name { get; private set; } = default!;
        public string? ExchangeType { get; set; }
        public bool? Durable { get; set; }
        public bool? AutoDelete { get; set; }
        public Dictionary<string, object?>? Arguments { get; set; }
        public ExchangeDeclareAttribute(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            Name = name;
        }
    }
}
