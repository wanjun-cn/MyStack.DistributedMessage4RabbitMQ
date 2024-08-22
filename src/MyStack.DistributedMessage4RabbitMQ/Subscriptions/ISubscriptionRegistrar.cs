using System.Collections.Generic;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ.Subscriptions
{
    internal interface ISubscriptionRegistrar
    {
        void Register(List<SubscriptionInfo> subscriptions);
    }
}
