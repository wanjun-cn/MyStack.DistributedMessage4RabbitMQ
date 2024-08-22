using Microsoft.Extensions.DistributedMessage4RabbitMQ;
using MyStack.DistributedMessage4RabbitMQ.Shared;

namespace MyStack.DistributedMessage4RabbitMQ.Consumer
{
    public class WrappedDataEventHandler : IDistributedEventHandler<WrappedEvent<WrappedData>>
    {
        public async Task HandleAsync(WrappedEvent<WrappedData> eventData, CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;
            Console.WriteLine("WrappedData");
        }
    }
}
