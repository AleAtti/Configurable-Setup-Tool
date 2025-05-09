namespace ServiceInstaller.model
{
        public class Package
        {
            public string Name { get; set; } = "";
            public string Type { get; set; } = "choco"; // choco, npm, zip, msi
            public string Arguments { get; set; } = "";
            public string Source { get; set; } = ""; // e.g. for offline zip/msi
            public string TargetDir { get; set; } = ""; // optional
        }
}
