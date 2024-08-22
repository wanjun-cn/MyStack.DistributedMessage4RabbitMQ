using System;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ.Subscriptions
{
    internal class SubscriptionInfo
    {
        public SubscriptionInfo(Type eventType, Type eventHandlerType)
        {
            EventType = eventType;
            EventHandlerType = eventHandlerType;
        }
        public SubscriptionInfo(Type eventType, Type eventHandlerType, Type replyType)
        {
            EventType = eventType;
            EventHandlerType = eventHandlerType;
            ReplyType = replyType;
        }

        public Type EventType { get; }
        public Type EventHandlerType { get; }
        public Type? ReplyType { get; }
    }
}
