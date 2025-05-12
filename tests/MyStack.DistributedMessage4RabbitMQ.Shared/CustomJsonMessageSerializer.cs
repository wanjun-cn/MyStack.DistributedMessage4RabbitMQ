using System;
using Microsoft.Extensions.DistributedMessage4RabbitMQ;
using Newtonsoft.Json;

namespace MyStack.DistributedMessage4RabbitMQ.Producer
{
    public class CustomJsonMessageSerializer : JsonMessageSerializer
    {
        public override object? Deserialize(string data, Type type)
        {
            var setting = new JsonSerializerSettings();

            return JsonConvert.DeserializeObject(data, type);
        }
        public override T? Deserialize<T>(string data) where T : default
        {
            return base.Deserialize<T>(data);
        }
        public override string Serialize<T>(T obj)
        {
            return base.Serialize(obj);
        }
    }
}
