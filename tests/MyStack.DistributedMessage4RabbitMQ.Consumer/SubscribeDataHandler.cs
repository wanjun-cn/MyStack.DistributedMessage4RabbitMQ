using Microsoft.Extensions.DistributedMessage4RabbitMQ;

namespace MyStack.DistributedMessage4RabbitMQ.Consumer
{
    public class SubscribeDataHandler : IDistributedEventHandler
    {
        public async Task HandleAsync(object eventData, CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;
        }
    }
}
