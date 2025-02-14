using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    public class ExchangeDeclareValueProvider
    {
        public ExchangeDeclareValue GetValue([NotNull] Type messageType, [NotNull] ExchangeDeclareValue defaultValue)
        {
            if (messageType.IsGenericType && messageType.GetGenericTypeDefinition() == typeof(DistributedEventWrapper<>))
                messageType = messageType.GetGenericArguments()[0];

            var exchangeDeclareAttribute = messageType!.GetCustomAttribute<ExchangeDeclareAttribute>();
            if (exchangeDeclareAttribute == null)
                return defaultValue;
            ExchangeDeclareValue declareValue = new()
            {
                Name = !string.IsNullOrEmpty(exchangeDeclareAttribute.Name) ? exchangeDeclareAttribute.Name : defaultValue.Name ?? MyStackConsts.DEFAULT_EXCHANGE_NAME,
                ExchangeType = !string.IsNullOrEmpty(exchangeDeclareAttribute.ExchangeType) ? exchangeDeclareAttribute.ExchangeType : defaultValue.ExchangeType ?? "topic",
                Durable = exchangeDeclareAttribute.Durable ?? defaultValue.Durable,
                AutoDelete = exchangeDeclareAttribute.AutoDelete ?? defaultValue.AutoDelete,
                Arguments = exchangeDeclareAttribute.Arguments == null ? exchangeDeclareAttribute.Arguments : defaultValue.Arguments
            };
            return declareValue;
        }
    }
}
