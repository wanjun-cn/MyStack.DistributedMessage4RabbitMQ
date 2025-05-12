using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    /// <summary>
    /// Represents an interface for distributed event handling services.
    /// </summary>
    /// <typeparam name="TDistributedEvent">Indicates the type of the distributed event.</typeparam>
    public interface IDistributedEventHandler<TDistributedEvent>
        where TDistributedEvent : class, IDistributedEvent
    {
        /// <summary>
        /// The task for handling the event.
        /// </summary>
        /// <param name="eventData">The distributed event object.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task HandleAsync(TDistributedEvent eventData, CancellationToken cancellationToken = default);
    }
}
