using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    /// <summary>
    /// Represents a distributed event handler interface
    /// </summary>
    /// <typeparam name="TDistributedEvent">The type of the distributed event</typeparam>
    public interface IDistributedEventHandler<TDistributedEvent>
        where TDistributedEvent : class, IDistributedEvent
    {
        /// <summary>
        /// The task of handle a message
        /// </summary>
        /// <param name="eventData">The message object</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns></returns>
        Task HandleAsync(TDistributedEvent eventData, CancellationToken cancellationToken = default);
    }
}
