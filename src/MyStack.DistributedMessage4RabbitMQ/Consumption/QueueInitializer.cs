using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DistributedMessage4RabbitMQ.Configuration;
using Microsoft.Extensions.DistributedMessage4RabbitMQ.Contracts;
using Microsoft.Extensions.DistributedMessage4RabbitMQ.Subscription;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ.Consumption
{
    public class QueueInitializer
    {
        private readonly List<string> _queueNames = new();
        protected ISubscriptionService SubscriptionService { get; }
        protected ExchangeDeclareValueProvider ExchangeDeclareValueProvider { get; }
        protected QueueDeclareValueProvider QueueDeclareValueProvider { get; }
        protected QueueBindValueProvider QueueBindValueProvider { get; }
        protected RabbitMQOptions Options { get; }
        protected ILogger Logger { get; }
        public QueueInitializer(
            ISubscriptionService subscriptionService,
            ExchangeDeclareValueProvider exchangeDeclareValueProvider,
            QueueDeclareValueProvider queueDeclareValueProvider,
            QueueBindValueProvider queueBindValueProvider,
            IOptions<RabbitMQOptions> options,
            ILogger<QueueInitializer> logger)
        {
            SubscriptionService = subscriptionService;
            ExchangeDeclareValueProvider = exchangeDeclareValueProvider;
            QueueDeclareValueProvider = queueDeclareValueProvider;
            QueueBindValueProvider = queueBindValueProvider;
            Options = options.Value;
            Logger = logger;
        }

        public virtual void BuildQueues(IModel channel)
        {
            var messageTypes = SubscriptionService.GetAllMessageTypes();
            if (messageTypes?.Count > 0)
            {
                foreach (var messageType in messageTypes)
                {
                    BuildQueue(channel, messageType);
                }
            }
        }

        protected virtual void BuildDeadLetter(IModel channel, Type messageType, ref string? deadLetterExchange, ref string? deadLetterRoutingKey)
        {
            var deadLetterAttribute = messageType.GetCustomAttribute<DeadLetterAttribute>();
            if (deadLetterAttribute == null)
                return;
            var deadLetterMessageType = deadLetterAttribute.MessageType;
            BuildQueue(channel, deadLetterMessageType, false);
            var queueBindValue = QueueBindValueProvider.GetValue(deadLetterMessageType);
            deadLetterExchange = queueBindValue.ExchangeName;
            deadLetterRoutingKey = queueBindValue.RoutingKey;
        }

        protected virtual void BuildQueue(IModel channel, Type messageType, bool isConsume = true)
        {
            var exchangeDeclareValue = ExchangeDeclareValueProvider.GetValue(messageType, Options.ExchangeOptions);
            var queueDeclareValue = QueueDeclareValueProvider.GetValue(messageType, Options.QueueOptions);
            var queueBindValue = QueueBindValueProvider.GetValue(messageType);

            string? deadLetterExchange = null, deadLetterRoutingKey = null;
            BuildDeadLetter(channel, messageType, ref deadLetterExchange, ref deadLetterRoutingKey);

            queueDeclareValue.Arguments ??= new Dictionary<string, object?>();
            if (!string.IsNullOrEmpty(deadLetterExchange))
                queueDeclareValue.Arguments["x-dead-letter-exchange"] = deadLetterExchange;
            if (!string.IsNullOrEmpty(deadLetterRoutingKey))
                queueDeclareValue.Arguments["x-dead-letter-routing-key"] = deadLetterRoutingKey;

            channel.QueueDeclare(queueBindValue.QueueName, queueDeclareValue.Durable, queueDeclareValue.Exclusive, queueDeclareValue.AutoDelete, queueDeclareValue.Arguments);
            channel.ExchangeDeclare(queueBindValue.ExchangeName, exchangeDeclareValue.ExchangeType, exchangeDeclareValue.Durable, exchangeDeclareValue.AutoDelete, exchangeDeclareValue.Arguments);
            channel.QueueBind(queueBindValue.QueueName, queueBindValue.ExchangeName, queueBindValue.RoutingKey);

            Logger.LogInformation("Binding routing key `{@RoutingKey}` to queue `{@QueueName}`", queueBindValue.RoutingKey, queueBindValue.QueueName);

            if (isConsume)
                _queueNames.Add(queueBindValue.QueueName);
        }
        public virtual IReadOnlyList<string> GetQueueNames() => _queueNames.AsReadOnly();
    }
}
