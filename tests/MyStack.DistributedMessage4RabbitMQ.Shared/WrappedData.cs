using Microsoft.Extensions.DistributedMessage4RabbitMQ.Contracts;

namespace MyStack.DistributedMessage4RabbitMQ.Shared
{
    [QueueDeclare("WrappedData")]
    [QueueBind("WrappedData")]
    [MessageHeader("A", "A")]
    public class WrappedData
    {

    }
}
