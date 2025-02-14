namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    public class QueueBindValue
    {
        public QueueBindValue(string queue, string exchange, string routingKey)
        {
            Queue = queue;
            Exchange = exchange;
            RoutingKey = routingKey;
        }

        public string Queue { get; set; } = default!;
        public string Exchange { get; set; } = default!;
        public string RoutingKey { get; set; } = default!;
    }
}
