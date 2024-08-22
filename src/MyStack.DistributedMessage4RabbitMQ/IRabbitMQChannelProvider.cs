using RabbitMQ.Client;
using System;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    public interface IRabbitMQChannelProvider : IDisposable
    {
        IModel CreateModel();
    }
}
