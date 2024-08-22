using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    public class DefaultMetadataResolver : IMetadataResolver
    {
        public Dictionary<string, object> GetMetadata(object eventData)
        {
            if (eventData == null)
                throw new ArgumentNullException(nameof(eventData), "事件数据不能为空");
            var metadataAttributes = eventData.GetType().GetCustomAttributes<EventMetadataAttribute>();
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
