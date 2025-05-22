using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DistributedMessage4RabbitMQ.Configuration;
using Microsoft.Extensions.DistributedMessage4RabbitMQ.Contracts;
using Microsoft.Extensions.DistributedMessage4RabbitMQ.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    public class RabbitMQDistributedMessageBus : IDistributedMessageBus
    {
        protected RabbitMQConnectionProvider RabbitMQProvider { get; }
        protected QueueBindValueProvider QueueBindValueProvider { get; }
        protected RoutingKeyProvider RoutingKeyProvider { get; }
        protected ILogger<RabbitMQDistributedMessageBus> Logger { get; }
        protected RabbitMQOptions Options { get; }
        protected IMessageSerializer MessageSerializer { get; }
        public RabbitMQDistributedMessageBus(RabbitMQConnectionProvider rabbitMQProvider,
            QueueBindValueProvider queueBindValueProvider,
            RoutingKeyProvider routingKeyProvider,
            ILogger<RabbitMQDistributedMessageBus> logger,
            IOptions<RabbitMQOptions> optionsAccessor,
            IMessageSerializer messageSerializer)
        {
            RabbitMQProvider = rabbitMQProvider;
            QueueBindValueProvider = queueBindValueProvider;
            RoutingKeyProvider = routingKeyProvider;
            Logger = logger;
            Options = optionsAccessor.Value;
            MessageSerializer = messageSerializer;
        }
        protected virtual void SetHeaders(object eventData, IBasicProperties basicProperties, MessageMetadata? metadata)
        {
            var headersAttributes = eventData.GetType().GetCustomAttributes<MessageHeaderAttribute>();
            if (headersAttributes != null)
            {
                basicProperties.Headers ??= new Dictionary<string, object?>();
                foreach (var headersAttribute in headersAttributes)
                {
                    basicProperties.Headers.TryAdd(headersAttribute.Key, headersAttribute.Value);
                }
            }
            if (eventData is IHasMessageMetadata hasMessageMetadata)
            {
                var metadataAttributes = hasMessageMetadata.Metadata.Where(x => x.Key.StartsWith(MyStackConsts.MESSAGE_HEADER));
                if (metadataAttributes.Any())
                {
                    foreach (var metadataAttribute in metadataAttributes)
                    {
                        var headerKey = metadataAttribute.Key.Replace(MyStackConsts.MESSAGE_HEADER, "");
                        basicProperties.Headers.TryAdd(headerKey, metadataAttribute.Value);
                    }
                }
            }
            if (metadata != null)
            {
                foreach (var kv in metadata.Where(x => x.Key.StartsWith(MyStackConsts.MESSAGE_HEADER)))
                {
                    var headerKey = kv.Key.Replace(MyStackConsts.MESSAGE_HEADER, "");
                    basicProperties.Headers.TryAdd(headerKey, kv.Value);
                }
            }
        }

        public virtual async Task PublishAsync(object eventData, MessageMetadata? metadata = null, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using var connection = await RabbitMQProvider.GetConnectionAsync(cancellationToken);
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
            var sendData = MessageSerializer.Serialize(messageData);
            var sendBytes = Encoding.UTF8.GetBytes(sendData);
            var basicProperties = channel.CreateBasicProperties();
            SetHeaders(eventData, basicProperties, metadata);

            // Publish message
            var queueBindValue = QueueBindValueProvider.GetValue(eventData.GetType());
            channel.BasicPublish(queueBindValue.ExchangeName, queueBindValue.RoutingKey, true, basicProperties, sendBytes);
            Logger?.LogInformation("[{@RoutingKey}] Published message: {@SendData}.", queueBindValue.RoutingKey, sendData);
            await Task.CompletedTask;
        }

        public virtual async Task<TRpcResponse?> SendAsync<TRpcResponse>(IRpcRequest<TRpcResponse> requestData, MessageMetadata? metadata = null, CancellationToken cancellationToken = default) where TRpcResponse : class
        {
            cancellationToken.ThrowIfCancellationRequested();
            using var connection = await RabbitMQProvider.GetConnectionAsync(cancellationToken);
            using var channel = connection.CreateModel();
            if (requestData == null)
                throw new ArgumentNullException(nameof(requestData), "Event data cannot be null");
            if (metadata != null)
            {
                foreach (var key in metadata.Keys)
                    requestData.Metadata.TryAdd(key, metadata[key]);
            }

            BlockingCollection<string> responseMessages = new BlockingCollection<string>();
            var queueBindValue = QueueBindValueProvider.GetValue(requestData.GetType());
            var exchangeName = queueBindValue.ExchangeName;
            var routingKey = queueBindValue.RoutingKey;

            // Set the reply queue and routing key
            var replyQueueName = channel.QueueDeclare().QueueName;
            var basicProperties = channel.CreateBasicProperties();
            basicProperties.ReplyTo = Guid.NewGuid().ToString();
            basicProperties.CorrelationId = Guid.NewGuid().ToString();
            basicProperties.Headers = new Dictionary<string, object>();
            SetHeaders(requestData, basicProperties, metadata);

            channel.QueueBind(replyQueueName, exchangeName, basicProperties.ReplyTo);
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (_, ea) =>
            {
                var replyMessage = Encoding.UTF8.GetString(ea.Body.Span);
                if (ea.BasicProperties.CorrelationId == basicProperties.CorrelationId)
                {
                    responseMessages.Add(replyMessage);
                }
                Logger?.LogInformation("[{@RoutingKey}] Received reply: {@ReplyMessage}.", routingKey, replyMessage);
            };
            channel.BasicConsume(queue: replyQueueName, autoAck: true, consumer: consumer);

            // Publish message to the queue
            var sendData = MessageSerializer.Serialize(requestData);
            var sendBytes = Encoding.UTF8.GetBytes(sendData);
            channel.BasicPublish(
                exchange: exchangeName,
                routingKey: routingKey,
                basicProperties: basicProperties,
                body: sendBytes);
            Logger?.LogInformation("[{@RoutingKey}] Published message: {@SendData}.", routingKey, sendData);

            // Get the RPC request timeout
            int millisecondsTimeout = -1;
            if (metadata != null && metadata.TryGetValue("RPCTimeout", out var timeoutValue))
            {
                millisecondsTimeout = Convert.ToInt32(timeoutValue);
            }
            else
            {
                millisecondsTimeout = Options.RPCTimeout;
            }
            // Get the RPC response message
            if (!responseMessages.TryTake(out var responseMessage, millisecondsTimeout, cancellationToken))
                throw new RpcTimeoutException("Response timeout when sending RPC request", millisecondsTimeout, routingKey);
            return MessageSerializer.Deserialize<TRpcResponse>(responseMessage);
        }
    }
}
