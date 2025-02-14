using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    public class QueueDeclareValueProvider
    {
        public QueueDeclareValue GetValue([NotNull] Type messageType, [NotNull] QueueDeclareValue defaultValue)
        {
            if (messageType.IsGenericType && messageType.GetGenericTypeDefinition() == typeof(DistributedEventWrapper<>))
                messageType = messageType.GetGenericArguments()[0];
            var queueDeclareAttribute = messageType.GetCustomAttribute<QueueDeclareAttribute>();
            if (queueDeclareAttribute == null)
                return defaultValue;
            QueueDeclareValue declareValue = new()
            {
                Name = !string.IsNullOrEmpty(queueDeclareAttribute.Name) ? queueDeclareAttribute.Name : defaultValue.Name ?? MyStackConsts.DEFAULT_QUEUE_NAME,
                Durable = queueDeclareAttribute.Durable ?? defaultValue.Durable,
                Exclusive = queueDeclareAttribute.Exclusive ?? defaultValue.Exclusive,
                AutoDelete = queueDeclareAttribute.AutoDelete ?? defaultValue.AutoDelete,
                Arguments = queueDeclareAttribute.Arguments == null ? queueDeclareAttribute.Arguments : defaultValue.Arguments
            };
            return declareValue;
        }
    }
}
