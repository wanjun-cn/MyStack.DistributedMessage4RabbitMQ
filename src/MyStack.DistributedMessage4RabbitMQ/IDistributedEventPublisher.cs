﻿using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    /// <summary>
    /// Represents an interface for distributed event publishing services.
    /// </summary>
    public interface IDistributedEventPublisher
    {
        /// <summary>
        /// Publishes a distributed event.
        /// </summary>
        /// <param name="eventData">The distributed event object.</param>
        /// <param name="metadata">The metadata of the message.
        /// <para>If the key of the metadata is prefixed with <see cref="MyStackConsts.RABBITMQ_HEADER"/> ("rabbitmq."), the message header will be set in the RabbitMQ request header.</para>
        /// </param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task PublishAsync(object eventData, MessageMetadata? metadata = null, CancellationToken cancellationToken = default);
    }
}
