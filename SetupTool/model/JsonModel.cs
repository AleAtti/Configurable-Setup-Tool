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
        public static T FromJson(string json)
        {
            return JsonSerializer.Deserialize<T>(json);
        }

        public static string ToJson(T obj)
        {
            return JsonSerializer.Serialize(obj);
        }
    }
}
