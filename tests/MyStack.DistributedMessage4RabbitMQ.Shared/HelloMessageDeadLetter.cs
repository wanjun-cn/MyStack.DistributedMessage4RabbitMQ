using Microsoft.Extensions.DistributedMessage4RabbitMQ;
using Microsoft.Extensions.DistributedMessage4RabbitMQ.Contracts;

namespace MyStack.DistributedMessage4RabbitMQ.Shared
{
    [QueueBind("HelloMessageDeadLetter", QueueName = "HelloDeadLetter", ExchangeName = "DeadLetter")]
    public class HelloMessageDeadLetter : DistributedEventBase
    {
    }
}
