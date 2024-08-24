using Microsoft.Extensions.DistributedMessage4RabbitMQ;

namespace MyStack.DistributedMessage4RabbitMQ.Consumer
{
    [Subscribe("ABC")]
    public class SubscribeDataHandler : IDistributedEventHandler
    {
        public async Task HandleAsync(object eventData, CancellationToken cancellationToken = default)
        {
            Console.WriteLine("SubscribeData");
            await Task.CompletedTask;
        }
    }
}
