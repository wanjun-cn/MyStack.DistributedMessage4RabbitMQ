using Microsoft.Extensions.DistributedMessage4RabbitMQ;
using Microsoft.Extensions.DistributedMessage4RabbitMQ.Contracts;

namespace MyStack.DistributedMessage4RabbitMQ.Shared
{
    [QueueBind("HelloMessage", QueueName = "Hello")]
    [DeadLetter(messageType: typeof(HelloMessageDeadLetter))]
    [MessageHeader("x-max-retry-count", "3")]
    public class HelloMessage : DistributedEventBase
    {
        public string Message { get; set; } = default!;
    }
}


