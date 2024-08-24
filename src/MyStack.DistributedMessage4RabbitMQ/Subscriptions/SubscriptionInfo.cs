using System;
using System.Reflection;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ.Subscriptions
{
    internal class SubscriptionInfo
    {
        public SubscriptionInfo(Type messageType, Type interfaceHandlerType)
        {
            MessageType = messageType;
            InterfaceHandlerType = interfaceHandlerType;
        }
        public SubscriptionInfo(string messageKey, Type interfaceHandlerType)
        {
            MessageKey = messageKey;
            InterfaceHandlerType = interfaceHandlerType;
        }
        public SubscriptionInfo(Type messageType, Type interfaceHandlerType, Type responseType)
        {
            MessageType = messageType;
            InterfaceHandlerType = interfaceHandlerType;
            ResponseType = responseType;
        } 
        public string? MessageKey { get; }
        public Type? MessageType { get; }
        public Type InterfaceHandlerType { get; }
        public Type? ResponseType { get; }
    }
}
