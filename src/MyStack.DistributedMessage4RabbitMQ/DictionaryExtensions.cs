using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ
{
    public static class DictionaryExtensions
    {
        /// <summary>  
        /// Tries to retrieve a value from the dictionary by the specified key.  
        /// If the value is a byte array, it will be converted to a UTF-8 string.  
        /// </summary>  
        /// <param name="dictionary">The dictionary to search in.</param>  
        /// <param name="key">The key to look for.</param>  
        /// <param name="value">The output value if the key is found.</param>  
        /// <returns>True if the key exists in the dictionary; otherwise, false.</returns>  
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

        /// <summary>  
        /// Tries to add a new key-value pair to the dictionary or updates the value if the key already exists.  
        /// </summary>  
        /// <param name="dictionary">The dictionary to modify.</param>  
        /// <param name="key">The key to add or update.</param>  
        /// <param name="value">The value to associate with the key.</param>  
        /// <returns>True if the operation is successful; otherwise, false.</returns>  
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
