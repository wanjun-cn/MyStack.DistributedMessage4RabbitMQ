using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ.Internal
{
    /// <summary>
    /// 实现默认的元数据解析器
    /// </summary>
    internal class DefaultMetadataResolver : IMetadataResolver
    {
        public Dictionary<string, object> GetMetadata(object messageData)
        {
            if (messageData == null)
                throw new ArgumentNullException(nameof(messageData), "事件数据不能为空");
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
