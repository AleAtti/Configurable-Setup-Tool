using System;
using System.Diagnostics;
using System.IO;
using SetupTool.model;

namespace SetupTool.Core
{
    public sealed class Removal
    {
        private readonly PackageManifest _manifest;

        public Removal(PackageManifest manifest)
        {
            _manifest = manifest ?? throw new ArgumentNullException(nameof(manifest));
        }

        public void RunRemoval()
        {
            foreach (var pkg in _manifest.OnlinePackages)
            {
                if (pkg.Type == PackageType.Choco)
                    UninstallPackage(pkg.Name);
            }

            RemoveWindowsService("PM2AutoStart");
            string scriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "pm2-startup.cmd");
            if (File.Exists(scriptPath)) File.Delete(scriptPath);

            Console.WriteLine("\nRemoval completed.");
        }

        private void UninstallPackage(string name)
        {
            Console.WriteLine($"Uninstalling {name} ...");
            Process.Start(new ProcessStartInfo
            {
                FileName = "choco",
                Arguments = $"uninstall {name} -y",
                UseShellExecute = true,
                Verb = "runas"
            })?.WaitForExit();
        }

        private void RemoveWindowsService(string serviceName)
        {
            Console.WriteLine($"Removing Windows service: {serviceName} ...");
            Process.Start(new ProcessStartInfo
            {
                FileName = "sc.exe",
                Arguments = $"delete {serviceName}",
                UseShellExecute = true,
                Verb = "runas"
            })?.WaitForExit();
        }
    }
}
