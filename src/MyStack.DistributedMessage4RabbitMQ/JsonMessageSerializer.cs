using System;
using Newtonsoft.Json;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ;

public class JsonMessageSerializer : IMessageSerializer
{
    public virtual T? Deserialize<T>(string data)
    {
        return JsonConvert.DeserializeObject<T>(data);
    }
    public virtual object? Deserialize(string data, Type type)
    {
        return JsonConvert.DeserializeObject(data, type);
    }
    public virtual string Serialize<T>(T obj)
    {
        return JsonConvert.SerializeObject(obj);
    }
}
