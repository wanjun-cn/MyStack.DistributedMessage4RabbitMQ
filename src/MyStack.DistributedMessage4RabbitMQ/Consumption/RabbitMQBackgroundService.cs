using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DistributedMessage4RabbitMQ.Configuration;
using Microsoft.Extensions.DistributedMessage4RabbitMQ.Subscription;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ.Consumption
{
    /// <summary>
    /// Represents the RabbitMQ background service for subscribing to message processing logic.
    /// </summary>
    internal class RabbitMQBackgroundService : BackgroundService
    {
        private readonly RabbitMQConnectionProvider _connectionProvider;
        private readonly RabbitMQOptions _options;
        private readonly ISubscriptionService _subscriptionService;
        private readonly QueueInitializer _queueInitializer;
        private readonly DistributedEventHandler _distributedEventHandler;
        private readonly RpcRequestHandler _rpcRequestHandler;
        private readonly ILogger<RabbitMQBackgroundService> _logger;
        public RabbitMQBackgroundService(RabbitMQConnectionProvider connectionProvider,
                IOptions<RabbitMQOptions> options,
                ISubscriptionService subscriptionService,
                QueueInitializer queueInitializer,
                DistributedEventHandler distributedEventHandler,
                RpcRequestHandler rpcRequestHandler,
                ILogger<RabbitMQBackgroundService> logger)
        {
            _connectionProvider = connectionProvider;
            _options = options.Value;
            _subscriptionService = subscriptionService;
            _queueInitializer = queueInitializer;
            _distributedEventHandler = distributedEventHandler;
            _rpcRequestHandler = rpcRequestHandler;
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var channel = await _connectionProvider.CreateChannelAsync(cancellationToken);
            _queueInitializer.BuildQueues(channel);
            channel.BasicQos(prefetchSize: 0, prefetchCount: _options.PrefetchCount, global: false);
            await RegisterConsumesAsync(channel, _queueInitializer.GetQueueNames(), cancellationToken);
        }

      

        private async Task RegisterConsumesAsync(IModel channel, IReadOnlyList<string> queueNames, CancellationToken cancellationToken)
        {
            if (queueNames.Count == 0)
                return;

            foreach (var queueName in queueNames.Distinct())
            {
                var consumer = new EventingBasicConsumer(channel);
                channel.BasicConsume(queueName, autoAck: false, consumer: consumer);

                consumer.Received += async (ch, eventArgs) =>
                {
                    var stopwatch = Stopwatch.StartNew();
                    try
                    {
                        if (_subscriptionService.IsRpcRequest(eventArgs.RoutingKey))
                        {
                            await _rpcRequestHandler.HandleAsync(channel, eventArgs, cancellationToken);
                        }
                        else if (_subscriptionService.IsEvent(eventArgs.RoutingKey))
                        {
                            await _distributedEventHandler.HandleAsync(channel, eventArgs, cancellationToken);
                        }
                        else
                        {
                            _logger.LogWarning("Received message with unknown routing key: {RoutingKey}.", eventArgs.RoutingKey);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "An error occurred while processing the message with routing key: {RoutingKey}.", eventArgs.RoutingKey);
                    }
                    finally
                    {
                        stopwatch.Stop();
                        _logger.LogInformation("Processed message with routing key: {RoutingKey} in {Elapsed}.", eventArgs.RoutingKey, stopwatch.Elapsed);
                    }
                };

                await Task.CompletedTask;
            }
        }
    }
}
