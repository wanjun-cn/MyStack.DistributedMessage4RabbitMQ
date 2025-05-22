using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DistributedMessage4RabbitMQ.Contracts;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ.Configuration
{
    /// <summary>
    /// Represents a service for providing exchange values.
    /// </summary>
    public class ExchangeDeclareValueProvider
    {
        /// <summary>
        /// Retrieves the exchange value from the message type.
        /// </summary>
        /// <param name="messageType">The message type.</param>
        /// <param name="defaultValue">The default exchange value.</param>
        /// <returns></returns>
        public ExchangeDeclareValue GetValue([NotNull] Type messageType, [NotNull] ExchangeDeclareValue defaultValue)
        {
            if (messageType.IsGenericType && messageType.GetGenericTypeDefinition() == typeof(DistributedEventWrapper<>))
                messageType = messageType.GetGenericArguments()[0];

            var exchangeDeclareAttribute = messageType!.GetCustomAttribute<ExchangeDeclareAttribute>();
            var xArgumentAttributes = messageType.GetCustomAttributes<XArgumentAttribute>().Where(x => x.Type == XArgumentType.ExchangeDeclare);
            ExchangeDeclareValue declareValue = new()
            {
                Name = !string.IsNullOrEmpty(exchangeDeclareAttribute?.Name) ? exchangeDeclareAttribute.Name : defaultValue.Name ?? MyStackConsts.DEFAULT_EXCHANGE_NAME,
                ExchangeType = !string.IsNullOrEmpty(exchangeDeclareAttribute?.ExchangeType) ? exchangeDeclareAttribute.ExchangeType : defaultValue.ExchangeType ?? "topic",
                Durable = exchangeDeclareAttribute?.Durable ?? defaultValue.Durable,
                AutoDelete = exchangeDeclareAttribute?.AutoDelete ?? defaultValue.AutoDelete,
                Arguments = new Dictionary<string, object?>()
            };
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
