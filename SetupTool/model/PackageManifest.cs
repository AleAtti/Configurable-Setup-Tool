using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SetupTool.model
{
    public class PackageManifest
    {
        public List<Package> OfflinePackages { get; set; } = new List<Package>();
        public List<Package> OnlinePackages { get; set; } = new List<Package>();
    }
}
