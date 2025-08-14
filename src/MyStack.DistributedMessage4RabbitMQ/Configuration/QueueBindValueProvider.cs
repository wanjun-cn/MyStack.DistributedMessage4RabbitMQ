using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DistributedMessage4RabbitMQ.Contracts;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ.Configuration
{
    /// <summary>
    /// Represents a service for providing queue binding values.
    /// </summary>
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
        /// <summary>
        /// Retrieves the queue binding value from the message type.
        /// </summary>
        /// <param name="messageType">The message type.</param>
        /// <returns></returns>
        public QueueBindValue GetValue([NotNull] Type messageType)
        {
            if (messageType.IsGenericType && messageType.GetGenericTypeDefinition() == typeof(DistributedEventWrapper<>))
                messageType = messageType.GetGenericArguments()[0];

            var exchangeDeclareValue = _exchangeDeclareValueProvider.GetValue(messageType, _options.ExchangeOptions);
            var queueDeclareValue = _queueDeclareValueProvider.GetValue(messageType, _options.QueueOptions);
            var queueBindAttribute = messageType.GetCustomAttribute<QueueBindAttribute>();
            var xArgumentAttributes = messageType.GetCustomAttributes<XArgumentAttribute>().Where(x => x.Type == XArgumentType.QueueBind);
            // 构建路由键，当未设置自定义路由键时，则使用消息类型的完全限定名
            var routingKey = !string.IsNullOrEmpty(queueBindAttribute?.RoutingKey) ? queueBindAttribute.RoutingKey : messageType.FullName!;
            // 检测是否启用路由键前缀，启用时追加前缀
            if (queueBindAttribute?.EnablePrefix ?? true)
                routingKey = _routingKeyProvider.GetValue(routingKey);
            QueueBindValue queueBindValue = new()
            {
                QueueName = (!string.IsNullOrEmpty(queueBindAttribute?.QueueName) ? queueBindAttribute.QueueName : queueDeclareValue.Name ?? MyStackConsts.DEFAULT_QUEUE_NAME)!,
                ExchangeName = (!string.IsNullOrEmpty(queueBindAttribute?.ExchangeName) ? queueBindAttribute.ExchangeName : exchangeDeclareValue.Name ?? MyStackConsts.DEFAULT_EXCHANGE_NAME)!,
                RoutingKey = routingKey,
                Arguments = new Dictionary<string, object?>()
            };
            if (xArgumentAttributes != null)
            {
                foreach (var xArgument in xArgumentAttributes)
                    queueBindValue.Arguments.TryAdd(xArgument.Key, xArgument.Value);
            }
            return queueBindValue;
        }
    }
}
