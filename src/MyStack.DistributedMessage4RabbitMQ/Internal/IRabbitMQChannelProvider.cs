using RabbitMQ.Client;
using System;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ.Internal
{
    /// <summary>
    /// Represents a RabbitMQ message channel provider
    /// </summary>
    internal interface IRabbitMQChannelProvider : IDisposable
    {
        /// <summary>
        /// Creates a channel object
        /// </summary>
        /// <returns></returns>
        IModel CreateModel();
    }
}