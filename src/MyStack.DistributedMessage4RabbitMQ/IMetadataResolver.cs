using System.Collections.Generic;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    /// <summary>
    /// 表示元数据解析器
    /// </summary>
    public interface IMetadataResolver
    {
        /// <summary>
        /// 获取事件对象的元数据字典
        /// </summary>
        /// <param name="messageData">事件对象</param>
        /// <returns></returns>
        Dictionary<string, object> GetMetadata(object messageData);
    }
}
