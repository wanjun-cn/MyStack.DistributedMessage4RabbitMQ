using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DistributedMessage4RabbitMQ.Serialization;
using Microsoft.Extensions.DistributedMessage4RabbitMQ.Subscription;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ.Consumption
{
    public class RpcRequestHandler
    {
        protected IServiceProvider ServiceProvider { get; }
        protected IMessageSerializer MessageSerializer { get; }
        protected ISubscriptionService SubscriptionService { get; }
        protected ILogger<RpcRequestHandler> Logger { get; }
        protected RoutingKeyProvider RoutingKeyProvider { get; }
        public RpcRequestHandler(IServiceProvider serviceProvider,
            IMessageSerializer messageSerializer,
            ISubscriptionService subscriptionService,
            ILogger<RpcRequestHandler> logger,
            RoutingKeyProvider routingKeyProvider)
        {
            ServiceProvider = serviceProvider;
            MessageSerializer = messageSerializer;
            SubscriptionService = subscriptionService;
            Logger = logger;
            RoutingKeyProvider = routingKeyProvider;
        }
        public virtual async Task HandleAsync(IModel channel, BasicDeliverEventArgs eventArgs, CancellationToken cancellationToken)
        {
            var receivedMessage = Encoding.UTF8.GetString(eventArgs.Body.Span);
            Logger?.LogInformation("Received RPC message: {@ReceivedMessage}.", receivedMessage);
            if (string.IsNullOrEmpty(receivedMessage))
                return;
            var messageTypes = SubscriptionService.GetMessageTypes(eventArgs.RoutingKey);
            if (messageTypes == null)
                throw new InvalidOperationException($"No message types found for the routing key `{eventArgs.RoutingKey}`.");
            if (messageTypes.Count > 1)
                throw new InvalidOperationException($"Multiple message types found for the same routing key `{eventArgs.RoutingKey}`.");
            var messageType = messageTypes[0];
            object? eventData = MessageSerializer.Deserialize(receivedMessage, messageType);
            if (eventData == null)
                return;

            string replyMessage = "";
            try
            {
                var responseType = messageType.GetInterfaces().Where(x => x.GetGenericTypeDefinition() == typeof(IRpcRequest<>)).SelectMany(x => x.GetGenericArguments()).FirstOrDefault();
                if (responseType == null)
                    return;
                var requestHandlerType = typeof(IRpcRequestHandler<,>).MakeGenericType(messageType, responseType);
                var requestHandler = ServiceProvider.GetRequiredService(requestHandlerType);
                var replyMessageObj = await ((dynamic)requestHandler).HandleAsync((dynamic)eventData, cancellationToken);
                replyMessage = MessageSerializer.Serialize(replyMessageObj);
            }
            finally
            {
                var properties = eventArgs.BasicProperties;
                var replyProperties = channel.CreateBasicProperties();
                replyProperties.CorrelationId = properties.CorrelationId;
                var replyBytes = Encoding.UTF8.GetBytes(replyMessage);
                var replyRoutingKey = RoutingKeyProvider.GetValue(properties.ReplyTo);
                //_logger?.LogInformation(@"[{RoutingKey}]Reply message: {ReplyMessage}.", replyRoutingKey, replyMessage);
                channel.BasicPublish(exchange: eventArgs.Exchange, routingKey: properties.ReplyTo, mandatory: false, basicProperties: replyProperties, body: replyBytes);
                channel.BasicAck(eventArgs.DeliveryTag, false);
            }
        }
    }
}
