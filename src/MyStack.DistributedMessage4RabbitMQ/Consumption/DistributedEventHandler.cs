using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DistributedMessage4RabbitMQ.Configuration;
using Microsoft.Extensions.DistributedMessage4RabbitMQ.Serialization;
using Microsoft.Extensions.DistributedMessage4RabbitMQ.Subscription;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ.Consumption
{
    public class DistributedEventHandler
    {
        protected IServiceProvider ServiceProvider { get; }
        protected ILogger Logger { get; }
        protected ISubscriptionService SubscriptionService { get; }
        protected IMessageSerializer MessageSerializer { get; }
        protected RabbitMQOptions Options { get; }
        protected RetryHandler RetryHandle { get; }
        public DistributedEventHandler(IServiceProvider serviceProvider,
            ILogger<DistributedEventHandler> logger,
            ISubscriptionService subscriptionService,
            IMessageSerializer messageSerializer,
            IOptions<RabbitMQOptions> options,
            RetryHandler retryHandle)
        {
            ServiceProvider = serviceProvider;
            Logger = logger;
            SubscriptionService = subscriptionService;
            MessageSerializer = messageSerializer;
            Options = options.Value;
            RetryHandle = retryHandle;
        }
        public virtual async Task HandleAsync(IModel channel, BasicDeliverEventArgs eventArgs, CancellationToken cancellationToken)
        {
            var receivedMessage = Encoding.UTF8.GetString(eventArgs.Body.Span);
            Logger.LogInformation("Received event message: {@ReceivedMessage}.", receivedMessage);
            if (string.IsNullOrEmpty(receivedMessage))
                return;
            var messageTypes = SubscriptionService.GetMessageTypes(eventArgs.RoutingKey);
            if (messageTypes == null)
                return;
            foreach (var messageType in messageTypes)
            {
                try
                {
                    await HandleMessage(messageType, eventArgs, receivedMessage, cancellationToken);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "An error occurred while processing the message of type {MessageType}.", messageType.FullName);

                    await RetryHandle.HandleAsync(channel, eventArgs, messageType);
                }
            }
            channel.BasicAck(eventArgs.DeliveryTag, false);
        }
        protected virtual async Task HandleMessage(Type messageType, BasicDeliverEventArgs eventArgs, string receivedMessage, CancellationToken cancellationToken)
        {
            object? eventData = MessageSerializer.Deserialize(receivedMessage, messageType);
            if (eventData == null)
                return;
            if (typeof(IDistributedEvent).IsAssignableFrom(messageType))
            {
                var eventHandlerType = typeof(IDistributedEventHandler<>).MakeGenericType(messageType);
                var eventHandlers = ServiceProvider.GetServices(eventHandlerType);
                if (eventHandlers != null)
                {
                    foreach (var eventHandler in eventHandlers)
                    {
                        await ((dynamic)eventHandler!).HandleAsync((dynamic)eventData, cancellationToken);
                    }
                }
            }
            else if (messageType != null)
            {
                var eventWrapperType = typeof(DistributedEventWrapper<>).MakeGenericType(messageType);
                var eventHandlerType = typeof(IDistributedEventHandler<>).MakeGenericType(eventWrapperType);
                var eventHandlers = ServiceProvider.GetServices(eventHandlerType);
                if (eventHandlers != null)
                {
                    var eventWrapper = Activator.CreateInstance(eventWrapperType, eventData);
                    foreach (var eventHandler in eventHandlers)
                    {
                        await ((dynamic)eventHandler!).HandleAsync((dynamic)eventWrapper!, cancellationToken);
                    }
                }
            }
        }


    }
}
