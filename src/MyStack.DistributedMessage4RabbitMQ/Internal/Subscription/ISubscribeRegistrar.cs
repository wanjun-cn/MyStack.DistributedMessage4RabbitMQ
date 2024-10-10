using System.Collections.Generic;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ.Internal.Subscription
{
    internal interface ISubscribeRegistrar
    {
        void Register(List<SubscribeInfo> subscriptions);
    }
}
