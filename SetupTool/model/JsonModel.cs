using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;

namespace ServiceInstaller.model
{
    public class JsonModel<T>
    {

        private static JsonSerializerOptions _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        public static T FromJson(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("File not found", filePath);
            }

            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException(nameof(filePath), "JSON string cannot be null or empty");
            }
            return JsonSerializer.Deserialize<T>(filePath);
        }

        public static string ToJson(T obj)
        {
           if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj), "Object cannot be null");
            }
            return JsonSerializer.Serialize(obj);
        }

        public static string ToJson(T obj, string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("File not found", filePath);
            }
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj), "Object cannot be null");
            }
            File.WriteAllText(filePath, JsonSerializer.Serialize(obj));
            return JsonSerializer.Serialize(obj);
        }
    }
}
