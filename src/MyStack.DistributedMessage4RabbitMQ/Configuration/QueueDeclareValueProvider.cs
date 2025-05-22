using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DistributedMessage4RabbitMQ.Contracts;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ.Configuration
{
    /// <summary>
    /// Represents the service for providing queue definition values.
    /// </summary>    
    public class QueueDeclareValueProvider
    {
        public QueueDeclareValue GetValue([NotNull] Type messageType, [NotNull] QueueDeclareValue defaultValue)
        {
            if (messageType.IsGenericType && messageType.GetGenericTypeDefinition() == typeof(DistributedEventWrapper<>))
                messageType = messageType.GetGenericArguments()[0];
            var queueDeclareAttribute = messageType.GetCustomAttribute<QueueDeclareAttribute>();

            QueueDeclareValue declareValue = new()
            {
                Name = !string.IsNullOrEmpty(queueDeclareAttribute?.Name) ? queueDeclareAttribute.Name : defaultValue.Name ?? MyStackConsts.DEFAULT_QUEUE_NAME,
                Durable = queueDeclareAttribute?.Durable ?? defaultValue.Durable,
                Exclusive = queueDeclareAttribute?.Exclusive ?? defaultValue.Exclusive,
                AutoDelete = queueDeclareAttribute?.AutoDelete ?? defaultValue.AutoDelete,
                Arguments = new Dictionary<string, object?>()
            };

            var xArgumentAttributes = messageType.GetCustomAttributes<XArgumentAttribute>().Where(x => x.Type == XArgumentType.QueueDeclare);

            if (xArgumentAttributes != null)
            {
                foreach (var xArgument in xArgumentAttributes)
                    declareValue.Arguments.TryAdd(xArgument.Key, xArgument.Value);
            }
            if (defaultValue.Arguments != null)
            {
                foreach (var kv in defaultValue.Arguments)
                    declareValue.Arguments.TryAdd(kv.Key, kv.Value);
                return declareValue;
            }
            return declareValue;
        }
    }
}
