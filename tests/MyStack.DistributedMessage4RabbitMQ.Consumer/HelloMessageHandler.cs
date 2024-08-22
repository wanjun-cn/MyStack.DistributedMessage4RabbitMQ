using MyStack.DistributedMessage4RabbitMQ.Shared;

namespace MyStack.DistributedMessage4RabbitMQ.Consumer
{
    public class HelloMessageHandler : MessageHandlerBase<HelloMessage>
    {

        protected async override Task InternalHandleAsync(HelloMessage eventData, CancellationToken cancellationToken = default)
        {
            Console.WriteLine("Hello");
            await Task.CompletedTask;
        }
    }
}
