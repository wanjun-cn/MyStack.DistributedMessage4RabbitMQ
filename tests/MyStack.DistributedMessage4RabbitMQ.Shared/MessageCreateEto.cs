using Microsoft.Extensions.DistributedMessage4RabbitMQ;
using Microsoft.Extensions.DistributedMessage4RabbitMQ.Contracts;

namespace MyStack.DistributedMessage4RabbitMQ.Shared
{
    [QueueBind("Multiway.Logistics.Eto.MessageManagement.MessageCreateEto", QueueName = "IdentityMessage")]
    [DeadLetter(messageType: typeof(MessageCreateEtoDeadLetter))]
    public class MessageCreateEto : DistributedEventBase
    {
        public string? MissionId { get; set; }
        public string Title { get; set; } = default!;
        public string Content { get; set; } = default!;
        public int Type { get; set; }
        public int Source { get; set; }
        public int PushType { get; set; }
    }
    [QueueBind("Multiway.Logistics.Eto.MessageManagement.MessageCreateEtoDeadLetter", QueueName = "IdentityMessageDeadLetter", ExchangeName = "MessageDeadLetter")]
    // [XArgument(XArgumentType.QueueDeclare, "x-max-length", 1_000)]
    // [XArgument(XArgumentType.QueueDeclare, "x-message-ttl", 86_400_000)]
    public class MessageCreateEtoDeadLetter : DistributedEventBase
    {
    }
}
