using System.Text.Json.Serialization;

namespace SetupTool.model
{
    public enum PackageType
    {
        Choco,
        Npm,
        Zip,
        Msi
    }

    public class Package
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";
        [JsonPropertyName("type")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PackageType Type { get; set; } = PackageType.Choco;
        [JsonPropertyName("arguments")]
        public string Arguments { get; set; } = null;
        [JsonPropertyName("source")]
        public string Source { get; set; } = null; // e.g. for offline zip/msi
        [JsonPropertyName("targetDir")]
        public string TargetDir { get; set; } = null; // optional
    }
}
