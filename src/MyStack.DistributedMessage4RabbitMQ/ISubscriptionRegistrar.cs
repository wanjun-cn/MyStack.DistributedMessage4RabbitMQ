using System;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    internal interface ISubscriptionRegistrar
    {
        void Subscribe(params Type[] messagetypes);
    }
}
