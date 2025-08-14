using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DistributedMessage4RabbitMQ;
using Microsoft.Extensions.DistributedMessage4RabbitMQ.Configuration;
using Microsoft.Extensions.DistributedMessage4RabbitMQ.Consumption;
using Microsoft.Extensions.DistributedMessage4RabbitMQ.Serialization;
using Microsoft.Extensions.DistributedMessage4RabbitMQ.Subscription;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Adds distributed messaging (RabbitMQ) to the service collection.
        /// </summary>
        /// <param name="services">The dependency service collection interface.</param>
        /// <param name="configure">The RabbitMQ configuration.</param>
        /// <param name="assemblies">The assemblies for registering message subscriptions.</param>
        /// <returns></returns>
        public static IServiceCollection AddDistributedMessage4RabbitMQ(this IServiceCollection services, Action<RabbitMQOptions> configure, params Assembly[] assemblies)
        {

            var messageTypes = new List<Type>();
            var eventHandlerTypes = assemblies.SelectMany(x => x.GetTypes().Where(x => !x.IsAbstract && x.IsPublic && x.GetInterfaces().Any(y => y.IsGenericType && y.GetGenericTypeDefinition() == typeof(IDistributedEventHandler<>))));
            if (eventHandlerTypes.Any())
            {
                foreach (var eventHandlerType in eventHandlerTypes)
                {
                    var handlerInterfaces = eventHandlerType.GetInterfaces()
                        .Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IDistributedEventHandler<>));
                    foreach (var handlerInterface in handlerInterfaces)
                    {
                        services.AddTransient(typeof(IDistributedEventHandler<>).MakeGenericType(handlerInterface.GetGenericArguments()), eventHandlerType);
                        messageTypes.Add(handlerInterface.GetGenericArguments()[0]);
                    }
                }
            }
            var requestHandlerTypes = assemblies.SelectMany(x => x.GetTypes().Where(x => !x.IsAbstract && x.IsPublic && x.GetInterfaces().Any(y => y.IsGenericType && y.GetGenericTypeDefinition() == typeof(IRpcRequestHandler<,>))));
            if (requestHandlerTypes.Any())
            {
                foreach (var requestHandlerType in requestHandlerTypes)
                {
                    var handlerInterfaces = requestHandlerType.GetInterfaces()
                        .Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IRpcRequestHandler<,>));
                    foreach (var handlerInterface in handlerInterfaces)
                    {
                        services.AddTransient(typeof(IRpcRequestHandler<,>).MakeGenericType(handlerInterface.GetGenericArguments()), requestHandlerType);
                        messageTypes.Add(handlerInterface.GetGenericArguments()[0]);
                    }
                }
            }

            services.AddTransient<QueueBindValueProvider>();
            services.AddTransient<ExchangeDeclareValueProvider>();
            services.AddTransient<QueueDeclareValueProvider>();
            services.AddTransient<RoutingKeyProvider>();
            services.AddTransient<RabbitMQConnectionProvider>();
            services.AddTransient<IDistributedMessageBus, RabbitMQDistributedMessageBus>();
            services.Configure(configure);
            services.AddSingleton<ISubscriptionRegistrar, SubscriptionService>();
            services.AddSingleton<ISubscriptionService>(factory =>
            {
                var subscriptionManager = factory.GetRequiredService<ISubscriptionRegistrar>();
                subscriptionManager.Subscribe(messageTypes.ToArray());
                return (SubscriptionService)subscriptionManager;
            });
            if (assemblies != null && assemblies.Length != 0)
                services.AddHostedService<RabbitMQBackgroundService>();

            services.AddTransient<IMessageSerializer, JsonMessageSerializer>();
            services.AddTransient<QueueInitializer>();
            services.AddSingleton<DistributedEventHandler>();
            services.AddSingleton<RpcRequestHandler>();
            services.AddTransient<RetryHandler>();

            return services;
        }
    }
}
