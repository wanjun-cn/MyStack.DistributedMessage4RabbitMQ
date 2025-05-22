using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DistributedMessage4RabbitMQ.Configuration;
using Microsoft.Extensions.DistributedMessage4RabbitMQ.Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ.Consumption
{
    public class RetryHandler
    {
        protected RabbitMQOptions Options { get; }
        protected ILogger Logger { get; }
        public RetryHandler(
            IOptions<RabbitMQOptions> options,
            ILogger<RetryHandler> logger)
        {
            Options = options.Value;
            Logger = logger;
        }
        public virtual async Task HandleAsync(IModel channel, BasicDeliverEventArgs eventArgs, Type messageType)
        {
            try
            {
                int maxRetryCount = Options.MaxRetryCount;
                if (eventArgs.BasicProperties.Headers != null && eventArgs.BasicProperties.Headers.TryGetHeaderValue("x-max-retry-count", out var maxRetryCountValue) && maxRetryCountValue != null)
                {
                    _ = int.TryParse(maxRetryCountValue.ToString(), out maxRetryCount);
                }
                if (maxRetryCount > 0)
                {
                    int retryCount = 0;
                    if (eventArgs.BasicProperties.Headers != null && eventArgs.BasicProperties.Headers.TryGetHeaderValue("x-retry-count", out var retryCountValue) && retryCountValue != null)
                    {
                        _ = int.TryParse(retryCountValue.ToString(), out retryCount);
                    }

                    if (retryCount <= maxRetryCount)
                    {
                        var properties = channel.CreateBasicProperties();
                        properties.Headers = eventArgs.BasicProperties.Headers;
                        properties.Headers ??= new Dictionary<string, object>();
                        if (properties.Headers.ContainsKey("x-retry-count"))
                            properties.Headers["x-retry-count"] = retryCount + 1;
                        else
                            properties.Headers.TryAdd("x-retry-count", retryCount + 1);
                        Logger.LogInformation("Retrying message with RoutingKey: {RoutingKey}, RetryCount: {RetryCount}, MaxRetryCount: {MaxRetryCount}.", eventArgs.RoutingKey, retryCount, maxRetryCount);
                        channel.BasicPublish(eventArgs.Exchange, eventArgs.RoutingKey, properties, eventArgs.Body.ToArray());
                        channel.BasicAck(eventArgs.DeliveryTag, false);
                    }
                    else
                        // If the max retry count is reached, send it to the dead letter queue or the original queue.
                        BasicNack(channel, eventArgs, messageType);
                }
                else
                {
                    // If the max retry count is 0, send it to the dead letter queue or the original queue.
                    BasicNack(channel, eventArgs, messageType);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "An error occurred while attempting retry. MessageType: {MessageType}, RoutingKey: {RoutingKey}.", messageType.Name, eventArgs.RoutingKey);
            }
            await Task.CompletedTask;
        }
        protected virtual void BasicNack(IModel channel, BasicDeliverEventArgs eventArgs, Type messageType)
        {
            var deadLetterAttribute = messageType.GetCustomAttribute<DeadLetterAttribute>();
            if (deadLetterAttribute != null && deadLetterAttribute.MessageType != null)
                channel.BasicNack(eventArgs.DeliveryTag, false, false);
            else
                channel.BasicNack(eventArgs.DeliveryTag, false, true);
        }
    }
}
