using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ.Internal
{
    /// <summary>
    /// Implements the default metadata resolver
    /// </summary>
    internal class DefaultMetadataResolver : IMetadataResolver
    {
        public Dictionary<string, object> GetMetadata(object messageData)
        {
            if (messageData == null)
                throw new ArgumentNullException(nameof(messageData), "Event data cannot be null");
            var metadataAttributes = messageData.GetType().GetCustomAttributes<MessageMetadataAttribute>();
            var metadata = new Dictionary<string, object>();
            if (metadataAttributes != null)
            {
                foreach (var metadataAttribute in metadataAttributes)
                {
                    metadata.TryAdd(metadataAttribute.Name, metadataAttribute.Value);
                }
            }
            return metadata;
        }
    }
}