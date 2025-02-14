using Microsoft.Extensions.DistributedMessage4RabbitMQ;

namespace MyStack.DistributedMessage4RabbitMQ.Shared
{
    [ExchangeDeclare("Hello")]
    [QueueDeclare("Hello")]
    [QueueBind("HelloMessage")]
    public class HelloMessage : DistributedEventBase
    {
        public string Message { get; set; } = default!;
    }
}


