using Microsoft.Extensions.DistributedMessage4RabbitMQ;

namespace MyStack.DistributedMessage4RabbitMQ.Shared
{
    [QueueDeclare("WrappedData")]
    [QueueBind("WrappedData")]
    [Headers("A", "A")]
    public class WrappedData
    {

    }
}
