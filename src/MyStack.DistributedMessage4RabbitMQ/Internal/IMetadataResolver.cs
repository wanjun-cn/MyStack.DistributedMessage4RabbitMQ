using System.Collections.Generic;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ.Internal
{
    /// <summary>
    /// Represents a metadata resolver
    /// </summary>
    internal interface IMetadataResolver
    {
        /// <summary>
        /// Gets the metadata dictionary for the event object
        /// </summary>
        /// <param name="messageData">The event object</param>
        /// <returns></returns>
        Dictionary<string, object> GetMetadata(object messageData);
    }
}