using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DistributedMessage4RabbitMQ.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    /// <summary>
    /// Implementation of the RabbitMQ distributed message bus
    /// </summary>
    public class RabbitMQDistributedMessageBus : IDistributedMessageBus
    {
        private readonly string? _routingKeyPrefix;
        private readonly string _exchangeName;
        private readonly IRoutingKeyResolver _routingKeyResolver;
        private readonly IRabbitMQChannelProvider _rabbitMQProvider;
        private readonly IMetadataResolver _metadataResolver;
        private readonly ILogger? _logger;
        public RabbitMQDistributedMessageBus(IServiceProvider serviceProvider)
        {
            _rabbitMQProvider = serviceProvider.GetRequiredService<IRabbitMQChannelProvider>();
            _routingKeyResolver = serviceProvider.GetRequiredService<IRoutingKeyResolver>();
            _metadataResolver = serviceProvider.GetRequiredService<IMetadataResolver>();
            Options = serviceProvider.GetRequiredService<IOptions<RabbitMQOptions>>().Value;
            _exchangeName = Options.ExchangeOptions.Name;
            _routingKeyPrefix = Options.RoutingKeyPrefix;
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            _logger = loggerFactory?.CreateLogger<RabbitMQDistributedMessageBus>();
        }
        protected RabbitMQOptions Options { get; private set; }
        private void AddHeaders(IBasicProperties basicProperties, object data, Dictionary<string, object>? headers = null)
        {
            basicProperties.Headers = new Dictionary<string, object>();
            var metadata = _metadataResolver.GetMetadata(data);
            if (metadata != null)
            {
                foreach (var kv in metadata)
                {
                    basicProperties.Headers.TryAdd(kv.Key, kv.Value);
                }
            }
            if (headers != null)
            {
                foreach (var kv in headers)
                {
                    basicProperties.Headers.TryAdd(kv.Key, kv.Value);
                }
            }
        }

        public async Task PublishAsync(string key, object eventData, Dictionary<string, object>? headers = null, CancellationToken cancellationToken = default)
        {
            using var channel = _rabbitMQProvider.CreateModel();
            cancellationToken.ThrowIfCancellationRequested();
            if (eventData == null)
                throw new ArgumentNullException(nameof(eventData), "Event data cannot be null");
            var sendData = JsonConvert.SerializeObject(eventData);
            var sendBytes = Encoding.UTF8.GetBytes(sendData);
            var basicProperties = channel.CreateBasicProperties();
            AddHeaders(basicProperties, eventData, headers);
      
            // Publish message
            channel.BasicPublish(_exchangeName, key, false, basicProperties, sendBytes);
            _logger?.LogInformation($"[{key}] Published message: {sendData}.");
            await Task.CompletedTask;
        }
        public async Task PublishAsync(IDistributedEvent eventData, Dictionary<string, object>? headers = null, CancellationToken cancellationToken = default)
        {
            var routingKey = _routingKeyResolver.GetRoutingKey(eventData.GetType());
            await PublishAsync(routingKey, eventData, headers, cancellationToken);
        }
        public async Task PublishAsync(object eventData, Dictionary<string, object>? headers = null, CancellationToken cancellationToken = default)
        {
            var routingKey = _routingKeyResolver.GetRoutingKey(eventData.GetType());
            await PublishAsync(routingKey, eventData, headers, cancellationToken);
        }
        public async Task<TRpcResponse?> SendAsync<TRpcResponse>(IRpcRequest<TRpcResponse> requestData, Dictionary<string, object>? headers = null, CancellationToken cancellationToken = default) where TRpcResponse : class
        {
            using var channel = _rabbitMQProvider.CreateModel();
            cancellationToken.ThrowIfCancellationRequested();
            if (requestData == null)
                throw new ArgumentNullException(nameof(requestData), "Event data cannot be null");
            BlockingCollection<string> responseMessages = new BlockingCollection<string>();
            var routingKey = _routingKeyResolver.GetRoutingKey(requestData.GetType());

            // Set the reply queue and routing key
            var replyQueueName = channel.QueueDeclare().QueueName;
            var basicProperties = channel.CreateBasicProperties();
            basicProperties.ReplyTo = _routingKeyResolver.GetRoutingKey(Guid.NewGuid().ToString());
            basicProperties.CorrelationId = Guid.NewGuid().ToString();
            basicProperties.Headers = new Dictionary<string, object>();
            AddHeaders(basicProperties, requestData, headers);
            
            channel.QueueBind(replyQueueName, _exchangeName, basicProperties.ReplyTo);
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
                exchange: _exchangeName,
                routingKey: routingKey,
                basicProperties: basicProperties,
                body: sendBytes);
            _logger?.LogInformation($"[{routingKey}] Published message: {sendData}.");

            // Get the RPC request timeout
            int millisecondsTimeout = -1;
            if (headers != null && headers.TryGetValue("RPCTimeout", out var timeoutValue))
            {
                millisecondsTimeout = Convert.ToInt32(timeoutValue);
            }
            else
            {
                millisecondsTimeout = Options.RPCTimeout;
            }
            // Get the RPC response message
            if (responseMessages.TryTake(out var responseMessage, millisecondsTimeout, cancellationToken))
                return JsonConvert.DeserializeObject<TRpcResponse>(responseMessage);
            return await Task.FromResult<TRpcResponse?>(default);
        }
    }
}