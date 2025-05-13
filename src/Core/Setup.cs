using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.NetworkInformation;
using SetupTool.model;

namespace SetupTool.Core
{
    public sealed class Setup
    {
        private readonly PackageManifest _manifest;

        public Setup(PackageManifest manifest)
        {
            _manifest = manifest ?? throw new ArgumentNullException(nameof(manifest));
        }

        public void RunSetup()
        {
            bool online = CheckInternetConnection();

            if (online)
            {
                EnsureChocolateyInstalled();
            }

            foreach (var pkg in _manifest.OnlinePackages)
            {
                switch (pkg.Type)
                {
                    case PackageType.Choco:
                        if (online) InstallPackage(pkg.Name);
                        break;
                    case PackageType.Npm:
                        if (online) InstallGlobalNpmPackage(pkg.Name);
                        else OfflineInstallNpmPackage(pkg);
                        break;
                    case PackageType.Zip:
                        OfflineUnzip(pkg);
                        break;
                    case PackageType.Msi:
                        OfflineInstallMsi(pkg);
                        break;
                }
            }

            string pm2StartScript = CreatePm2StartupScript();
            RegisterWindowsService("PM2AutoStart", pm2StartScript);
            Console.WriteLine("\nSetup completed.");
        }

        private bool CheckInternetConnection()
        {
            try
            {
                using (Ping ping = new Ping())
                {
                    var reply = ping.Send("8.8.8.8", 1000);
                    return reply?.Status == IPStatus.Success;
                }
            }
            catch
            {
                return false;
            }
        }

        private void EnsureChocolateyInstalled()
        {
            if (!File.Exists("C:\\ProgramData\\chocolatey\\bin\\choco.exe"))
            {
                Console.WriteLine("Installing Chocolatey...");
                Process.Start(new ProcessStartInfo
                {
                    FileName = "powershell",
                    Arguments = "-NoProfile -ExecutionPolicy Bypass -Command \"Set-ExecutionPolicy Bypass -Scope Process -Force; iex ((New-Object System.Net.WebClient).DownloadString('https://chocolatey.org/install.ps1'))\"",
                    UseShellExecute = true,
                    Verb = "runas"
                })?.WaitForExit();
            }
            else
            {
                Console.WriteLine("Chocolatey is already installed.");
            }
        }

        private void InstallPackage(string name)
        {
            Console.WriteLine($"Installing {name} ...");
            Process.Start(new ProcessStartInfo
            {
                FileName = "choco",
                Arguments = $"install {name} -y",
                UseShellExecute = true,
                Verb = "runas"
            })?.WaitForExit();
        }

        private void InstallGlobalNpmPackage(string name)
        {
            Console.WriteLine($"Installing NPM package {name} ...");
            Process.Start(new ProcessStartInfo
            {
                FileName = "npm",
                Arguments = $"install -g {name}",
                UseShellExecute = true,
                Verb = "runas"
            })?.WaitForExit();
        }

        private void OfflineInstallNpmPackage(Package pkg)
        {
            if (!File.Exists(pkg.Source)) return;
            Console.WriteLine($"Installing NPM package offline: {pkg.Name} ...");
            Process.Start(new ProcessStartInfo
            {
                FileName = "npm",
                Arguments = $"install -g \"{pkg.Source}\"",
                UseShellExecute = true,
                Verb = "runas"
            })?.WaitForExit();
        }

        private void OfflineUnzip(Package pkg)
        {
            if (!File.Exists(pkg.Source)) return;
            if (string.IsNullOrWhiteSpace(pkg.TargetDir)) return;
            if (Directory.Exists(pkg.TargetDir)) Directory.Delete(pkg.TargetDir, true);
            ZipFile.ExtractToDirectory(pkg.Source, pkg.TargetDir);
            Console.WriteLine($"Extracted {pkg.Name} to {pkg.TargetDir}");
        }

        private void OfflineInstallMsi(Package pkg)
        {
            if (!File.Exists(pkg.Source)) return;
            Console.WriteLine($"Installing MSI package {pkg.Name} ...");
            Process.Start(new ProcessStartInfo
            {
                FileName = pkg.Source,
                UseShellExecute = true,
                Verb = "runas"
            })?.WaitForExit();
        }

        private string CreatePm2StartupScript()
        {
            string scriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "pm2-startup.cmd");
            File.WriteAllText(scriptPath, "pm2 resurrect");
            return scriptPath;
        }

        private void RegisterWindowsService(string serviceName, string executablePath)
        {
            Console.WriteLine($"Registering Windows service: {serviceName} ...");
            Process.Start(new ProcessStartInfo
            {
                FileName = "sc.exe",
                Arguments = $"create {serviceName} binPath= \"cmd /c {executablePath}\" start= auto",
                UseShellExecute = true,
                Verb = "runas"
            })?.WaitForExit();
        }
    }
}
