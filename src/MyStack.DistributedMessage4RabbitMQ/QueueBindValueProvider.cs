using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    public class QueueBindValueProvider
    {
        private readonly ExchangeDeclareValueProvider _exchangeDeclareValueProvider;
        private readonly QueueDeclareValueProvider _queueDeclareValueProvider;
        private readonly RoutingKeyProvider _routingKeyProvider;
        private readonly RabbitMQOptions _options;
        public QueueBindValueProvider(ExchangeDeclareValueProvider exchangeDeclareValueProvider,
            QueueDeclareValueProvider queueDeclareValueProvider,
            IOptions<RabbitMQOptions> optionsAccessor,
            RoutingKeyProvider routingKeyProvider)
        {
            _exchangeDeclareValueProvider = exchangeDeclareValueProvider;
            _queueDeclareValueProvider = queueDeclareValueProvider;
            _options = optionsAccessor.Value;
            _routingKeyProvider = routingKeyProvider;
        }
        public QueueBindValue GetValue([NotNull] Type messageType)
        {
            if (messageType.IsGenericType && messageType.GetGenericTypeDefinition() == typeof(DistributedEventWrapper<>))
                messageType = messageType.GetGenericArguments()[0];

            var exchangeDeclareValue = _exchangeDeclareValueProvider.GetValue(messageType, _options.ExchangeOptions);
            var queueDeclareValue = _queueDeclareValueProvider.GetValue(messageType, _options.QueueOptions);
            var queueBindAttribute = messageType.GetCustomAttribute<QueueBindAttribute>();

            string queueName, exchangeName, eventName;
            if (queueBindAttribute != null)
            {
                queueName = (!string.IsNullOrEmpty(queueBindAttribute.QueueName) ? queueBindAttribute.QueueName : queueDeclareValue.Name ?? MyStackConsts.DEFAULT_QUEUE_NAME)!;
                exchangeName = (!string.IsNullOrEmpty(queueBindAttribute.ExchangeName) ? queueBindAttribute.ExchangeName : exchangeDeclareValue.Name ?? MyStackConsts.DEFAULT_EXCHANGE_NAME)!;
                eventName = queueBindAttribute.RoutingKey;
            }
            else
            {
                queueName = queueDeclareValue.Name ?? MyStackConsts.DEFAULT_QUEUE_NAME;
                exchangeName = exchangeDeclareValue.Name ?? MyStackConsts.DEFAULT_EXCHANGE_NAME;
                eventName = messageType.FullName!;
            }
            var routingKey = _routingKeyProvider.GetValue(eventName);
            return new QueueBindValue(queueName, exchangeName, routingKey);
        }
    }
}
