namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    /// <summary>
    /// Represents an interface for distributed events.
    /// </summary>
    public interface IDistributedEvent: IHasMessageMetadata
    {
    }
}
