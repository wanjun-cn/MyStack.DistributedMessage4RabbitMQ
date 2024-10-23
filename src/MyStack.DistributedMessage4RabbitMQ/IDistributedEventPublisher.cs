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
        /// Publishes an event
        /// </summary>
        /// <param name="key">The key name of the event</param>
        /// <param name="eventData">The event data</param>
        /// <param name="headers">The message header data</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns></returns>
        Task PublishAsync(string key, object eventData, Dictionary<string, object>? headers = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Publishes an event
        /// </summary>
        /// <param name="eventData">The event data</param>
        /// <param name="headers">The message header data</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns></returns>
        Task PublishAsync(IDistributedEvent eventData, Dictionary<string, object>? headers = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Publishes an event object
        /// </summary>
        /// <param name="eventData">The event data</param>
        /// <param name="headers">The message header data</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns></returns>
        Task PublishAsync(object eventData, Dictionary<string, object>? headers = null, CancellationToken cancellationToken = default);
    }
}