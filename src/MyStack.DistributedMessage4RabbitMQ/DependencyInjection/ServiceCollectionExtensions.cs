using Microsoft.Extensions.DistributedMessage4RabbitMQ;
using Microsoft.Extensions.DistributedMessage4RabbitMQ.Internal;
using Microsoft.Extensions.DistributedMessage4RabbitMQ.Internal.Subscription;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add RabbitMQ message bus
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="configure">Configuration delegate</param>
        /// <param name="assemblies">Assemblies to register event subscriptions</param>
        /// <returns>Returns the service collection</returns>
        public static IServiceCollection AddDistributedMessage4RabbitMQ(this IServiceCollection services, Action<RabbitMQOptions> configure, params Assembly[] assemblies)
        {
            var options = new RabbitMQOptions();
            configure?.Invoke(options);
            services.Configure(configure);
            services.AddTransient<IRabbitMQChannelProvider, DefaultRabbitMQChannelProvider>();
            services.AddTransient<IRpcMessageSender, RabbitMQDistributedMessageBus>();
            services.AddTransient<IDistributedEventPublisher, RabbitMQDistributedMessageBus>();
            services.AddTransient<IDistributedMessageBus, RabbitMQDistributedMessageBus>();
            services.AddTransient<IRoutingKeyResolver, DefaultRoutingKeyResolver>();
            services.AddTransient<IMetadataResolver, DefaultMetadataResolver>();
            services.AddHostedService<RabbitMQMessageListener>();

            List<SubscribeInfo> subscribers = new List<SubscribeInfo>();
            var eventHandlerTypes = assemblies.SelectMany(x => x.GetTypes().Where(x => !x.IsAbstract && x.IsPublic && x.GetInterfaces().Any(y => y.IsGenericType && y.GetGenericTypeDefinition() == typeof(IDistributedEventHandler<>) || y == typeof(IDistributedEventHandler))));
            if (eventHandlerTypes.Any())
            {
                foreach (var eventHandlerType in eventHandlerTypes)
                {
                    var handlerInterfaces = eventHandlerType.GetInterfaces()
                        .Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IDistributedEventHandler<>) || x == typeof(IDistributedEventHandler));
                    foreach (var handlerInterface in handlerInterfaces)
                    {
                        if (handlerInterface.IsGenericType)
                        {
                            if (handlerInterface.GetGenericArguments()[0].IsGenericType && handlerInterface.GetGenericArguments()[0].GetGenericTypeDefinition() == typeof(DistributedEventWrapper<>))
                            {
                                services.AddTransient(typeof(IDistributedEventHandler<>).MakeGenericType(handlerInterface.GetGenericArguments()), eventHandlerType);
                                subscribers.Add(new SubscribeInfo(handlerInterface.GetGenericArguments()[0].GetGenericArguments()[0], typeof(IDistributedEventHandler<>)));
                            }
                            else
                            {
                                services.AddTransient(typeof(IDistributedEventHandler<>).MakeGenericType(handlerInterface.GetGenericArguments()), eventHandlerType);
                                subscribers.Add(new SubscribeInfo(handlerInterface.GetGenericArguments()[0], typeof(IDistributedEventHandler<>)));
                            }
                        }
                        else
                        {
                            var subscribeAttribute = eventHandlerType.GetCustomAttribute<SubscribeAttribute>();
                            if (subscribeAttribute != null)
                            {
                                services.AddTransient(typeof(IDistributedEventHandler), eventHandlerType);
                                subscribers.Add(new SubscribeInfo(subscribeAttribute.Key, typeof(IDistributedEventHandler)));
                            }
                        }
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
                        subscribers.Add(new SubscribeInfo(handlerInterface.GetGenericArguments()[0], typeof(IRpcRequestHandler<,>), handlerInterface.GetGenericArguments()[1]));
                    }
                }
            }
            services.AddSingleton<ISubscribeRegistrar, SubscribeManager>();
            services.AddSingleton<ISubscribeManager>(factory =>
            {
                var subscribeRegistrar = factory.GetRequiredService<ISubscribeRegistrar>();
                subscribeRegistrar.Register(subscribers);
                return (SubscribeManager)subscribeRegistrar;
            });

            return services;
        }
    }
}