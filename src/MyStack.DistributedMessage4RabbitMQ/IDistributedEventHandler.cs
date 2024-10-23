using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    /// <summary>
    /// Represents a distributed event handler interface
    /// </summary>
    /// <typeparam name="TEvent">The type of the event</typeparam>
    public interface IDistributedEventHandler<TEvent>
        where TEvent : class, IDistributedEvent
    {
        /// <summary>
        /// Handles the event
        /// </summary>
        /// <param name="eventData">The event object</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns></returns>
        Task HandleAsync(TEvent eventData, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Represents a distributed event handler interface
    /// </summary>
    public interface IDistributedEventHandler
    {
        /// <summary>
        /// Handles the event
        /// </summary>
        /// <param name="eventData">The event object</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns></returns>
        Task HandleAsync(object eventData, CancellationToken cancellationToken = default);
    }
}