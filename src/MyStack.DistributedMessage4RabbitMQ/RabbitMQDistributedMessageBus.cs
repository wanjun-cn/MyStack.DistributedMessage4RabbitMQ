using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    public class RabbitMQDistributedMessageBus : IDistributedMessageBus
    {
        private readonly RabbitMQConnectionProvider _rabbitMQProvider;
        private readonly QueueBindValueProvider _queueBindValueProvider;
        private readonly RoutingKeyProvider _routingKeyProvider;
        private readonly ILogger<RabbitMQDistributedMessageBus> _logger;
        private readonly RabbitMQOptions _options;
        public RabbitMQDistributedMessageBus(RabbitMQConnectionProvider rabbitMQProvider,
        QueueBindValueProvider queueBindValueProvider,
        RoutingKeyProvider routingKeyProvider,
        ILogger<RabbitMQDistributedMessageBus> logger,
        IOptions<RabbitMQOptions> optionsAccessor)
        {
            _rabbitMQProvider = rabbitMQProvider;
            _queueBindValueProvider = queueBindValueProvider;
            _routingKeyProvider = routingKeyProvider;
            _logger = logger;
            _options = optionsAccessor.Value;
        }
        private void SetHeaders(object eventData, IBasicProperties basicProperties, MessageMetadata? metadata)
        {
            var headersAttributes = eventData.GetType().GetCustomAttributes<HeadersAttribute>();
            if (headersAttributes != null)
            {
                basicProperties.Headers ??= new Dictionary<string, object?>();
                foreach (var headersAttribute in headersAttributes)
                {
                    basicProperties.Headers.TryAdd(headersAttribute.Key, headersAttribute.Value);
                }
            }

            if (eventData is IDistributedEvent distributedEvent)
            {
                var metadataAttributes = distributedEvent.Metadata.Where(x => x.Key.StartsWith(MyStackConsts.RABBITMQ_HEADER));
                if (metadataAttributes.Any())
                {
                    foreach (var metadataAttribute in metadataAttributes)
                    {
                        var headerKey = metadataAttribute.Key.Replace(MyStackConsts.RABBITMQ_HEADER, "");
                        basicProperties.Headers.TryAdd(headerKey, metadataAttribute.Value);
                    }
                }
            }

            if (metadata != null)
            {
                foreach (var kv in metadata.Where(x => x.Key.StartsWith(MyStackConsts.RABBITMQ_HEADER)))
                {
                    var headerKey = kv.Key.Replace(MyStackConsts.RABBITMQ_HEADER, "");
                    basicProperties.Headers.TryAdd(headerKey, kv.Value);
                }
            }
        }

        public async Task PublishAsync(object eventData, MessageMetadata? metadata = null, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using var connection = await _rabbitMQProvider.GetConnectionAsync(cancellationToken);
            using var channel = connection.CreateModel();
            if (eventData == null)
                throw new ArgumentNullException(nameof(eventData), "Event data cannot be null");
            object? messageData = eventData;

            if (eventData is IDistributedEvent distributedEvent)
            {
                if (metadata != null)
                {
                    foreach (var key in metadata.Keys)
                        distributedEvent.Metadata.TryAdd(key, metadata[key]);
                }
            }
            else
            {
                var eventWrapperType = typeof(DistributedEventWrapper<>).MakeGenericType(eventData.GetType());
                messageData = Activator.CreateInstance(eventWrapperType, eventData);
                if (metadata != null)
                {
                    foreach (var key in metadata.Keys)
                        ((dynamic)messageData!).Metadata.TryAdd(key, metadata[key]);
                }
            }

            var sendData = JsonConvert.SerializeObject(messageData);
            var sendBytes = Encoding.UTF8.GetBytes(sendData);
            var basicProperties = channel.CreateBasicProperties();
            SetHeaders(eventData, basicProperties, metadata);

            // Publish message
            var queueBindValue = _queueBindValueProvider.GetValue(eventData.GetType());
            channel.BasicPublish(queueBindValue.ExchangeName, queueBindValue.RoutingKey, true, basicProperties, sendBytes);
            _logger?.LogInformation($"[{queueBindValue.RoutingKey}] Published message: {sendData}.");
            await Task.CompletedTask;
        }

        public async Task<TRpcResponse?> SendAsync<TRpcResponse>(IRpcRequest<TRpcResponse> requestData, MessageMetadata? metadata = null, CancellationToken cancellationToken = default) where TRpcResponse : class
        {
            cancellationToken.ThrowIfCancellationRequested();
            using var connection = await _rabbitMQProvider.GetConnectionAsync(cancellationToken);
            using var channel = connection.CreateModel();
            if (requestData == null)
                throw new ArgumentNullException(nameof(requestData), "Event data cannot be null");
            if (metadata != null)
            {
                foreach (var key in metadata.Keys)
                    requestData.Metadata.TryAdd(key, metadata[key]);
            }

            BlockingCollection<string> responseMessages = new BlockingCollection<string>();
            var queueBindValue = _queueBindValueProvider.GetValue(requestData.GetType());
            var exchangeName = queueBindValue.ExchangeName;
            var routingKey = queueBindValue.RoutingKey;

            // Set the reply queue and routing key
            var replyQueueName = channel.QueueDeclare().QueueName;
            var basicProperties = channel.CreateBasicProperties();
            basicProperties.ReplyTo = Guid.NewGuid().ToString();
            basicProperties.CorrelationId = Guid.NewGuid().ToString();
            basicProperties.Headers = new Dictionary<string, object>();
            SetHeaders(requestData, basicProperties, metadata);

            var replyRoutingKey = _routingKeyProvider.GetValue(basicProperties.ReplyTo);
            channel.QueueBind(replyQueueName, exchangeName, replyRoutingKey);
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (_, ea) =>
            {
                var replyMessage = Encoding.UTF8.GetString(ea.Body.Span);
                if (ea.BasicProperties.CorrelationId == basicProperties.CorrelationId)
                {
                    responseMessages.Add(replyMessage);
                }
                _logger?.LogInformation($"[{routingKey}] Received reply: {replyMessage}.");
            };
            channel.BasicConsume(queue: replyQueueName, autoAck: true, consumer: consumer);

            // Publish message to the queue
            var sendData = JsonConvert.SerializeObject(requestData);
            var sendBytes = Encoding.UTF8.GetBytes(sendData);
            channel.BasicPublish(
                exchange: exchangeName,
                routingKey: routingKey,
                basicProperties: basicProperties,
                body: sendBytes);
            _logger?.LogInformation($"[{routingKey}] Published message: {sendData}.");

            // Get the RPC request timeout
            int millisecondsTimeout = -1;
            if (metadata != null && metadata.TryGetValue("RPCTimeout", out var timeoutValue))
            {
                millisecondsTimeout = Convert.ToInt32(timeoutValue);
            }
            else
            {
                millisecondsTimeout = _options.RPCTimeout;
            }
            // Get the RPC response message
            if (!responseMessages.TryTake(out var responseMessage, millisecondsTimeout, cancellationToken))
                throw new RpcTimeoutException("Response timeout when sending RPC request", millisecondsTimeout, routingKey);
            return JsonConvert.DeserializeObject<TRpcResponse>(responseMessage);
        }
    }
}
