using Microsoft.Extensions.DistributedMessage4RabbitMQ;
using Microsoft.Extensions.DistributedMessage4RabbitMQ.Contracts;

namespace MyStack.DistributedMessage4RabbitMQ.Shared
{

    [MessageHeader("A", "A")]
    public class Ping : RpcRequestBase<Pong>
    {
        public string SendBy { get; set; } = default!;
    }
}

