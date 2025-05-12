using Microsoft.Extensions.DistributedMessage4RabbitMQ;

namespace MyStack.DistributedMessage4RabbitMQ.Shared
{

    [Headers("A", "A")]
    public class Ping : RpcRequestBase<Pong>
    {
        public string SendBy { get; set; } = default!;
    }
}

