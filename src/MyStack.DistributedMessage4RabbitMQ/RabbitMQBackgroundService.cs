using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    internal class RabbitMQBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly RabbitMQProvider _rabbitMQProvider;
        private readonly ExchangeDeclareValueProvider _exchangeDeclareValueProvider;
        private readonly QueueDeclareValueProvider _queueDeclareValueProvider;
        private readonly QueueBindValueProvider _queueBindValueProvider;
        private readonly RabbitMQOptions _options;
        private readonly ILogger<RabbitMQBackgroundService> _logger;
        private readonly SubscriptionManager _subscriptionManager;
        public RabbitMQBackgroundService(IServiceProvider serviceProvider,
                RabbitMQProvider rabbitMQProvider,
                ExchangeDeclareValueProvider exchangeDeclareValueProvider,
                QueueDeclareValueProvider queueDeclareValueProvider,
                QueueBindValueProvider queueBindValueProvider,
                IOptions<RabbitMQOptions> options,
                ILogger<RabbitMQBackgroundService> logger,
                SubscriptionManager subscriptionManager
            )
        {
            _serviceProvider = serviceProvider;
            _rabbitMQProvider = rabbitMQProvider;
            _exchangeDeclareValueProvider = exchangeDeclareValueProvider;
            _queueDeclareValueProvider = queueDeclareValueProvider;
            _queueBindValueProvider = queueBindValueProvider;
            _options = options.Value;
            _logger = logger;
            _subscriptionManager = subscriptionManager;
        }
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var connection = await _rabbitMQProvider.GetConnectionAsync(cancellationToken);
            var channel = connection.CreateModel();
            var queueNames = BindRoutingKeysAndGetQueues(channel);
            await ReceiveMessageAsync(channel, queueNames, cancellationToken);
        }
        private List<string> BindRoutingKeysAndGetQueues(IModel channel)
        {
            List<string> queueNames = new List<string>();
            var subscriptions = _subscriptionManager.GetAllSubscriptions();
            if (subscriptions == null || subscriptions.Count == 0) return queueNames;
            foreach (var messageType in subscriptions)
            {
                var exchangeDeclareValue = _exchangeDeclareValueProvider.GetValue(messageType, _options.ExchangeOptions);
                var queueDeclareValue = _queueDeclareValueProvider.GetValue(messageType, _options.QueueOptions);
                var queueBindValue = _queueBindValueProvider.GetValue(messageType);

                channel.QueueDeclare(queueBindValue.Queue, queueDeclareValue.Durable, queueDeclareValue.Exclusive, queueDeclareValue.AutoDelete, queueDeclareValue.Arguments);
                channel.ExchangeDeclare(queueBindValue.Exchange, exchangeDeclareValue.ExchangeType ?? "topic", exchangeDeclareValue.Durable, exchangeDeclareValue.AutoDelete, exchangeDeclareValue.Arguments);
                channel.QueueBind(queueBindValue.Queue, queueBindValue.Exchange, queueBindValue.RoutingKey, null);
                _logger?.LogInformation($"Binding routing key `{queueBindValue.RoutingKey}` to queue `{queueBindValue.Queue}`");
                queueNames.Add(queueBindValue.Queue);
            }

            channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
            return queueNames;
        }

        private async Task ReceiveMessageAsync(IModel channel, List<string> queueNames, CancellationToken cancellationToken)
        {
            if (queueNames.Count != 0)
            {
                foreach (var queueName in queueNames.Distinct())
                {
                    var consumer = new EventingBasicConsumer(channel);
                    channel.BasicConsume(queueName, autoAck: false, consumer: consumer);
                    consumer.Received += async (ch, ea) =>
                    {
                        var receivedMessage = Encoding.UTF8.GetString(ea.Body.Span);
                        _logger?.LogInformation($"Received message: {receivedMessage}.");

                        if (string.IsNullOrEmpty(receivedMessage))
                            return;
                        var subscriptions = _subscriptionManager.GetSubscriptions(ea.RoutingKey);
                        if (subscriptions == null)
                            return;
                        foreach (var subscription in subscriptions)
                        {
                            object? eventData = JsonConvert.DeserializeObject(receivedMessage, subscription);
                            if (eventData == null)
                                return;

                            if (subscription.GetInterfaces().Any(x => x == typeof(IDistributedEvent)))
                            {
                                await DistributedEventHandleAsync(channel, ea, subscription, eventData, cancellationToken);
                            }
                            else if (subscription.GetInterfaces().Any(x => x.GetGenericTypeDefinition() == typeof(IRpcRequest<>)))
                            {
                                await RpcMessageHandleAsync(channel, ea, subscription, eventData, cancellationToken);
                            }
                        }
                    };
                    await Task.CompletedTask;
                }
            }
        }
        private async Task RpcMessageHandleAsync(IModel channel, BasicDeliverEventArgs e, Type messageType, object eventData, CancellationToken cancellationToken)
        {
            string replyMessage = "";
            try
            {
                var responseType = messageType.GetInterfaces().Where(x => x.GetGenericTypeDefinition() == typeof(IRpcRequest<>)).SelectMany(x => x.GetGenericArguments()).FirstOrDefault();
                if (responseType == null)
                    return;
                var requestHandlerType = typeof(IRpcRequestHandler<,>).MakeGenericType(messageType, responseType);
                var requestHandler = _serviceProvider.GetRequiredService(requestHandlerType);
                var replyMessageObj = await ((dynamic)requestHandler).HandleAsync((dynamic)eventData, cancellationToken);
                replyMessage = JsonConvert.SerializeObject(replyMessageObj);
            }
            finally
            {
                var properties = e.BasicProperties;
                var replyProperties = channel.CreateBasicProperties();
                replyProperties.CorrelationId = properties.CorrelationId;
                _logger?.LogInformation($"Reply message: {replyMessage}.");
                var replyBytes = Encoding.UTF8.GetBytes(replyMessage);
                var queueBindValue = _queueBindValueProvider.GetValue(messageType);
                channel.BasicPublish(exchange: queueBindValue.Exchange, routingKey: properties.ReplyTo, mandatory: false, basicProperties: replyProperties, body: replyBytes);
                channel.BasicAck(e.DeliveryTag, false);
            }
        }

        private async Task DistributedEventHandleAsync(IModel channel, BasicDeliverEventArgs e, Type messageType, object eventData, CancellationToken cancellationToken)
        {
            if (typeof(IDistributedEvent).IsAssignableFrom(messageType))
            {
                var eventHandlerType = typeof(IDistributedEventHandler<>).MakeGenericType(messageType);
                var eventHandlers = _serviceProvider.GetServices(eventHandlerType);
                if (eventHandlers != null)
                {
                    foreach (var eventHandler in eventHandlers)
                    {
                        try
                        {
                            await ((dynamic)eventHandler!).HandleAsync((dynamic)eventData, cancellationToken);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, ex.Message);
                        }
                    }
                }
            }
            else if (messageType != null)
            {
                var eventWrapperType = typeof(DistributedEventWrapper<>).MakeGenericType(messageType);
                var eventHandlerType = typeof(IDistributedEventHandler<>).MakeGenericType(eventWrapperType);
                var eventHandlers = _serviceProvider.GetServices(eventHandlerType);
                if (eventHandlers != null)
                {
                    var eventWrapper = Activator.CreateInstance(eventWrapperType, eventData);
                    foreach (var eventHandler in eventHandlers)
                    {
                        try
                        {
                            await ((dynamic)eventHandler!).HandleAsync((dynamic)eventWrapper!, cancellationToken);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, ex.Message);
                        }
                    }
                }
            }

            channel.BasicAck(e.DeliveryTag, false);
        }
    }
}
