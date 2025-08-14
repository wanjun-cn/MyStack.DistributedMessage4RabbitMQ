using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    /// <summary>  
    /// Defines a handler for distributed events.  
    /// </summary>  
    /// <typeparam name="TDistributedEvent">The type of the distributed event.</typeparam>  
    public interface IDistributedEventHandler<TDistributedEvent>
        where TDistributedEvent : class, IDistributedEvent
    {
        /// <summary>  
        /// Handles the distributed event asynchronously.  
        /// </summary>  
        /// <param name="eventData">The event data to handle.</param>  
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>  
        /// <returns>A task that represents the asynchronous operation.</returns>  
        Task HandleAsync(TDistributedEvent eventData, CancellationToken cancellationToken = default);
    }
}
