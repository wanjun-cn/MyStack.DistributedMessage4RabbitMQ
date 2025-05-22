using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    public static class DictionaryExtensions
    {
        public static bool TryGetHeaderValue(this IDictionary<string, object> dictionary, string key, out object? value)
        {
            value = null;
            if (dictionary == null || !dictionary.TryGetValue(key, out value))
            {
                return false;
            }
            if (value is byte[] bytes)
            {
                value = Encoding.UTF8.GetString(bytes);
                return true;
            }
            return true;
        }

        public static bool TryAddOrUpdate(this IDictionary<string, object> dictionary, string key, object value)
        {
            if (dictionary == null) return false;
            if (dictionary.ContainsKey(key))
            {
                dictionary[key] = value;
                return true;
            }
            else
            {
                dictionary.Add(key, value);
                return true;
            }
        }
    }
}
