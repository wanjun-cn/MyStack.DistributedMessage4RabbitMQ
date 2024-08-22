using Microsoft.Extensions.DependencyInjection;
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
using System.Threading.Tasks;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    public class RabbitMQDistributedMessageBus : IDistributedMessageBus
    {
        private string? _routingKeyPrefix;

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
        public async Task PublishAsync(IDistributedEvent eventData, CancellationToken cancellationToken = default)
        {
            using var channel = _rabbitMQProvider.CreateModel();
            cancellationToken.ThrowIfCancellationRequested();
            if (eventData == null)
                throw new ArgumentNullException(nameof(eventData), "事件数据不能为空");
            // 生成路由键
            var routingKey = _routingKeyResolver.GetRoutingKey(eventData.GetType());
            var sendData = JsonConvert.SerializeObject(eventData);
            var sendBytes = Encoding.UTF8.GetBytes(sendData);
            var basicProperties = channel.CreateBasicProperties();
            basicProperties.Headers = new Dictionary<string, object>();
            var metadata = _metadataResolver.GetMetadata(eventData);
            if (metadata != null)
            {
                foreach (var kv in metadata)
                {
                    basicProperties.Headers.TryAdd(kv.Key, kv.Value);
                }
            }
            channel.BasicPublish(_exchangeName, routingKey, false, basicProperties, sendBytes);
            _logger?.LogInformation($"[{routingKey}]发布消息: {sendData}。");
            await Task.CompletedTask;
        }
        public async Task PublishAsync(object eventData, CancellationToken cancellationToken = default)
        {
            using var channel = _rabbitMQProvider.CreateModel();
            cancellationToken.ThrowIfCancellationRequested();
            if (eventData == null)
                throw new ArgumentNullException(nameof(eventData), "事件数据不能为空");
            // 生成路由键
            var routingKey = _routingKeyResolver.GetRoutingKey(eventData.GetType());
            var sendData = JsonConvert.SerializeObject(eventData);
            var sendBytes = Encoding.UTF8.GetBytes(sendData);
            var basicProperties = channel.CreateBasicProperties();
            basicProperties.Headers = new Dictionary<string, object>();
            var metadata = _metadataResolver.GetMetadata(eventData);
            if (metadata != null)
            {
                foreach (var kv in metadata)
                {
                    basicProperties.Headers.TryAdd(kv.Key, kv.Value);
                }
            }
            channel.BasicPublish(_exchangeName, routingKey, false, basicProperties, sendBytes);
            _logger?.LogInformation($"[{routingKey}]发布消息: {sendData}。");
            await Task.CompletedTask;
        }
        public async Task<TRpcResponse?> SendAsync<TRpcResponse>(IRpcRequest<TRpcResponse> eventData, CancellationToken cancellationToken = default) where TRpcResponse : class
        {
            using var channel = _rabbitMQProvider.CreateModel();
            cancellationToken.ThrowIfCancellationRequested();
            if (eventData == null)
                throw new ArgumentNullException(nameof(eventData), "事件数据不能为空");
            BlockingCollection<string> replyMessages = new BlockingCollection<string>();
            var replyQueueName = channel.QueueDeclare().QueueName;
            var routingKey = _routingKeyResolver.GetRoutingKey(eventData.GetType());
            var basicProperties = channel.CreateBasicProperties();

            basicProperties.ReplyTo = Guid.NewGuid().ToString();
            basicProperties.CorrelationId = Guid.NewGuid().ToString();
            basicProperties.Headers = new Dictionary<string, object>();
            var metadata = _metadataResolver.GetMetadata(eventData);
            if (metadata != null)
            {
                foreach (var kv in metadata)
                {
                    basicProperties.Headers.TryAdd(kv.Key, kv.Value);
                }
            }

            channel.QueueBind(replyQueueName, _exchangeName, $"{_routingKeyPrefix}{basicProperties.ReplyTo}");
            // 接收回复消息
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (_, ea) =>
            {
                var replyMessage = Encoding.UTF8.GetString(ea.Body.Span);
                if (ea.BasicProperties.CorrelationId == basicProperties.CorrelationId)
                {
                    replyMessages.Add(replyMessage);
                }
                _logger?.LogInformation($"[{routingKey}]收到回复: {replyMessage}。");
            };
            channel.BasicConsume(queue: replyQueueName, autoAck: true, consumer: consumer);


            // 发送消息
            var sendData = JsonConvert.SerializeObject(eventData);
            var sendBytes = Encoding.UTF8.GetBytes(sendData);
            channel.BasicPublish(
                exchange: _exchangeName,
                routingKey: routingKey,
                basicProperties: basicProperties,
                body: sendBytes);
            _logger?.LogInformation($"[{routingKey}]发布消息: {sendData}。");

            //  获取RPC请求的超时时间
            int millisecondsTimeout = -1;
            if (metadata != null && metadata.TryGetValue("RPCTimeout", out var timeoutValue))
            {
                millisecondsTimeout = Convert.ToInt32(timeoutValue);
            }
            else
            {
                millisecondsTimeout = Options.RPCTimeout;
            }

            if (replyMessages.TryTake(out var replyMessage, millisecondsTimeout, cancellationToken))
                return JsonConvert.DeserializeObject<TRpcResponse>(replyMessage);
            return await Task.FromResult<TRpcResponse?>(default);
        }
    }
}
