using System;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ.Subscription
{
    /// <summary>
    /// Represents an interface for subscription registration.
    /// </summary>
    public interface ISubscriptionRegistrar
    {
        /// <summary>
        /// Subscribes to a collection of message types.
        /// </summary>
        /// <param name="messagetypes"></param>
        void Subscribe(params Type[] messagetypes);
    }
}
