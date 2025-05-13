using System;
using System.IO;
using System.Text.Json;

namespace SetupTool.model
{
    public static class JsonModel<T>
    {
        private static readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        /// <summary>
        /// Deserializes an object of type T from a JSON string.
        /// </summary>
        /// <param name="json">The JSON string to deserialize.</param>
        /// <returns>An object of type T.</returns>
        public static T FromJsonString(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                throw new ArgumentNullException(nameof(json), "JSON string cannot be null or empty");

            return JsonSerializer.Deserialize<T>(json, _options);
        }

        /// <summary>
        /// Deserializes an object of type T from a JSON file.
        /// </summary>
        /// <param name="filePath">The path to the JSON file.</param>
        /// <returns>An object of type T.</returns>
        public static T FromJsonFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentNullException(nameof(filePath), "File path cannot be null or empty");

            if (!File.Exists(filePath))
                throw new FileNotFoundException("File not found", filePath);

            string json = File.ReadAllText(filePath);
            return FromJsonString(json);
        }

        /// <summary>
        /// Serializes an object of type T to a JSON string.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>A JSON string representation of the object.</returns>
        public static string ToJsonString(T obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj), "Object cannot be null");

            return JsonSerializer.Serialize(obj, _options);
        }

        /// <summary>
        /// Serializes an object of type T to a JSON file.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="filePath">The path to the JSON file.</param>
        public static void ToJsonFile(T obj, string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentNullException(nameof(filePath), "File path cannot be null or empty");

            if (obj == null)
                throw new ArgumentNullException(nameof(obj), "Object cannot be null");

            string json = ToJsonString(obj);
            File.WriteAllText(filePath, json);
        }
    }
}
