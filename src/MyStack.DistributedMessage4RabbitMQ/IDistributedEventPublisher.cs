using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    /// <summary>  
    /// Interface for publishing distributed events to RabbitMQ.  
    /// </summary>  
    public interface IDistributedEventPublisher
    {
        /// <summary>  
        /// Publishes an event asynchronously.  
        /// </summary>  
        /// <param name="eventData">The event data to be published.</param>  
        /// <param name="metadata">Optional metadata associated with the message.</param>  
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>  
        /// <returns>A task that represents the asynchronous publish operation.</returns>  
        Task PublishAsync(object eventData, MessageMetadata? metadata = null, CancellationToken cancellationToken = default);
    }
}
