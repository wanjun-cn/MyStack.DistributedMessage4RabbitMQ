using System;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ.Subscription
{
    internal class SubscribeInfo
    {
        public SubscribeInfo(Type messageType, Type interfaceHandlerType)
        {
            MessageType = messageType;
            InterfaceHandlerType = interfaceHandlerType;
        }
        public SubscribeInfo(string messageKey, Type interfaceHandlerType)
        {
            MessageKey = messageKey;
            InterfaceHandlerType = interfaceHandlerType;
        }
        public SubscribeInfo(Type messageType, Type interfaceHandlerType, Type responseType)
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
