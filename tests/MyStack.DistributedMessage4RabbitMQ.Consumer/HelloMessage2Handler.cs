using Microsoft.Extensions.Logging;
using MyStack.DistributedMessage4RabbitMQ.Shared;

namespace MyStack.DistributedMessage4RabbitMQ.Consumer
{
    public class HelloMessage2Handler : MessageHandlerBase<HelloMessage>
    {
        private readonly ILogger<HelloMessage2Handler> _logger;
        public HelloMessage2Handler(ILogger<HelloMessage2Handler> logger)
        {
            _logger = logger;
        }
        protected async override Task InternalHandleAsync(HelloMessage eventData, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Hello2 message: {eventData}");
            await Task.CompletedTask;
        }
    }
}
