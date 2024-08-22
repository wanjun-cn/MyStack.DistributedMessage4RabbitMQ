using System.Collections.Generic;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    public interface IMetadataResolver
    {
        Dictionary<string, object> GetMetadata(object eventData);
    }
}
