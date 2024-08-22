using Microsoft.Extensions.DistributedMessage4RabbitMQ;

namespace MyStack.DistributedMessage4RabbitMQ.Shared
{
    [EventMetadata("A", "A")]
    public class Ping : IRpcRequest<Pong>
    {
        public string SendBy { get; set; } = default!;
    }
}

