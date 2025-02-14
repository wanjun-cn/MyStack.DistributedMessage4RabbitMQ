using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    /// <summary>
    /// Represents a distributed event publisher interface
    /// </summary>
    public interface IDistributedEventPublisher
    {

        /// <summary>
        /// Publishes an distributed event object
        /// </summary>
        /// <param name="eventData">The event data</param>
        /// <param name="metadata">The metadata of message. 
        /// <para>If the key name starts with  <see cref="MyStackConsts.RABBITMQ_HEADER"/> ("rabbitmq."), it will be set to the header of RabbitMQ message</para>
        /// </param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns></returns>
        Task PublishAsync(object eventData, MessageMetadata? metadata = null, CancellationToken cancellationToken = default);
    }
}
