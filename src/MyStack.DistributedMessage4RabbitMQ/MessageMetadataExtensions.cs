namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    public static class MessageMetadataExtensions
    {
        public static MessageMetadata AddKeyValue(this MessageMetadata meta, string name, string value)
        {
            meta.TryAdd($"{MyStackConsts.RABBITMQ_HEADER}{name}", value);
            return meta;
        }
    }
}
