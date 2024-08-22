using Microsoft.Extensions.DistributedMessage4RabbitMQ;

namespace MyStack.DistributedMessage4RabbitMQ.Consumer
{
    public abstract class MessageHandlerBase<TMessage> : IDistributedEventHandler<TMessage> where TMessage : class, IDistributedEvent
    {
        protected MessageHandlerBase()
        {
        }
        protected abstract Task InternalHandleAsync(TMessage eventData, CancellationToken cancellationToken = default);
        public virtual async Task HandleAsync(TMessage eventData, CancellationToken cancellationToken = default)
        {
            try
            {
                await InternalHandleAsync(eventData, cancellationToken);
            }
            catch
            {

            }
        }
    }
}
