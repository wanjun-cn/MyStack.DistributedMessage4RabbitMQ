using Microsoft.Extensions.DistributedMessage4RabbitMQ;
using MyStack.DistributedMessage4RabbitMQ.Shared;

namespace MyStack.DistributedMessage4RabbitMQ.Consumer
{
    public class HelloMessageHandler : IDistributedEventHandler<HelloMessage>
    {
        public Task HandleAsync(HelloMessage eventData, CancellationToken cancellationToken = default)
        {
            throw new Exception("A");
        }
    }
}
