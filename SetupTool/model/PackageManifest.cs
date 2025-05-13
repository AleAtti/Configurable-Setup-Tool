using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;


namespace SetupTool.model
{
    public class PackageManifest
    {
        [JsonPropertyName("offlinePackages")]
        public List<Package> OfflinePackages { get; set; } = new List<Package>();
        [JsonPropertyName("onlinePackages")]
        public List<Package> OnlinePackages { get; set; } = new List<Package>();
    }
}
