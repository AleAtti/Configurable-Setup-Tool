using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace ServiceInstaller.model
{
    public class JsonModel<T>
    {

        private static JsonSerializerOptions _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        public static T FromJson(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                throw new ArgumentNullException(nameof(json), "JSON string cannot be null or empty");
            }
            return JsonSerializer.Deserialize<T>(json);
        }

        public static string ToJson(T obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj), "Object cannot be null");
            }
            return JsonSerializer.Serialize(obj);
        }
    }
}
