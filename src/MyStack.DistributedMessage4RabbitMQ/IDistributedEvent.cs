using System;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    /// <summary>
    /// Represents a distributed event interface
    /// </summary>
    public interface IDistributedEvent 
    {
        MessageMetadata Metadata { get; }
    }
}
