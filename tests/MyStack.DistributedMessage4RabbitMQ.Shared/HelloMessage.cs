using Microsoft.Extensions.DistributedMessage4RabbitMQ;

namespace MyStack.DistributedMessage4RabbitMQ.Shared
{
    [EventName("HelloMessage")]
    public class HelloMessage : IDistributedEvent
    {
        public string Message { get; set; } = default!;
    }
}


