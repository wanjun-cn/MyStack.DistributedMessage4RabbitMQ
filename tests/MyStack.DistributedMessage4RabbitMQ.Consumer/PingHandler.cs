using Microsoft.Extensions.DistributedMessage4RabbitMQ;
using MyStack.DistributedMessage4RabbitMQ.Shared;

namespace MyStack.DistributedMessage4RabbitMQ.Consumer
{
    public class PingHandler : IRpcRequestHandler<Ping, Pong>
    {

        public Task<Pong> HandleAsync(Ping message, CancellationToken cancellationToken = default)
        {
            Console.WriteLine("Ping");
            return Task.FromResult(new Pong() { ReplyBy = "B" });
        }
    }

}

