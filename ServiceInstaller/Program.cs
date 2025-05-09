using System;
using System.Diagnostics;
using System.IO;
using System.Net.NetworkInformation;
using System.IO.Compression;
using System.Text.Json;
using System.Collections.Generic;
using ServiceInstaller.model;

namespace ServiceInstaller
{
    partial class Program
    {
        static PackageManifest _manifest;
        static void Main(string[] args)
        {
            Console.WriteLine("--- Konfigurierbares Setup-Tool ---\n");
            string packagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "include\\packages.json");
            if (!File.Exists(packagePath))
            {
                Console.WriteLine("Fehlende Datei: packages.json");
                return;
            }

            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                string json = File.ReadAllText("./include/packages.json");
                _manifest = JsonSerializer.Deserialize<PackageManifest>(json, options);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fehler beim Lesen der Konfiguration: " + ex.Message);
                return;
            }

            if (args.Length == 0)
            {
                Console.WriteLine("Bitte gib 'setup' oder 'remove' als Argument an.");
                return;
            }

            string command = args[0].ToLower();

            if (command == "setup")
                RunSetup();
            else if (command == "remove")
                RunRemoval();
            else
                Console.WriteLine("Unbekanntes Argument. Bitte verwende 'setup' oder 'remove'.");
        }

        static void RunSetup()
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
                    case "choco":
                        if (online) InstallPackage(pkg.Name);
                        break;
                    case "npm":
                        if (online) InstallGlobalNpmPackage(pkg.Name);
                        else OfflineInstallNpmPackage(pkg);
                        break;
                    case "zip":
                        OfflineUnzip(pkg);
                        break;
                    case "msi":
                        OfflineInstallMsi(pkg);
                        break;
                }
            }

            //!TODO: Offline Implementation

            string pm2StartScript = CreatePm2StartupScript();
            RegisterWindowsService("PM2AutoStart", pm2StartScript);
            Console.WriteLine("\nSetup abgeschlossen.");
        }

        static void RunRemoval()
        {
            foreach (var pkg in _manifest.OnlinePackages)
            {
                if (pkg.Type == "choco")
                    UninstallPackage(pkg.Name);
            }

            RemoveWindowsService("PM2AutoStart");
            string scriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "pm2-startup.cmd");
            if (File.Exists(scriptPath)) File.Delete(scriptPath);

            Console.WriteLine("\nEntfernung abgeschlossen.");
        }

        static bool CheckInternetConnection()
        {
            try
            {
                using (Ping ping = new Ping())
                {
                    var reply = ping.Send("8.8.8.8", 1000);
                    return reply?.Status == IPStatus.Success;
                }
                
            }
            catch { return false; }
        }

        static void EnsureChocolateyInstalled()
        {
            if (!File.Exists("C:\\ProgramData\\chocolatey\\bin\\choco.exe"))
            {
                Console.WriteLine("Chocolatey wird installiert...");
                Process.Start(new ProcessStartInfo
                {
                    FileName = "powershell",
                    Arguments = "-NoProfile -ExecutionPolicy Bypass -Command \"Set-ExecutionPolicy Bypass -Scope Process -Force; iex ((New-Object System.Net.WebClient).DownloadString('https://chocolatey.org/install.ps1'))\"",
                    UseShellExecute = true,
                    Verb = "runas"
                })?.WaitForExit();
            }
            else Console.WriteLine("Chocolatey ist bereits installiert.");
        }

        static void InstallPackage(string name)
        {
            Console.WriteLine($"Installiere {name} ...");
            Process.Start(new ProcessStartInfo
            {
                FileName = "choco",
                Arguments = $"install {name} -y",
                UseShellExecute = true,
                Verb = "runas"
            })?.WaitForExit();
        }

        static void UninstallPackage(string name)
        {
            Console.WriteLine($"Deinstalliere {name} ...");
            Process.Start(new ProcessStartInfo
            {
                FileName = "choco",
                Arguments = $"uninstall {name} -y",
                UseShellExecute = true,
                Verb = "runas"
            })?.WaitForExit();
        }

        static void InstallGlobalNpmPackage(string name)
        {
            Console.WriteLine($"Installiere NPM-Paket {name} ...");
            Process.Start(new ProcessStartInfo
            {
                FileName = "npm",
                Arguments = $"install -g {name}",
                UseShellExecute = true,
                Verb = "runas"
            })?.WaitForExit();
        }

        static void OfflineInstallNpmPackage(Package pkg)
        {
            if (!File.Exists(pkg.Source)) return;
            Console.WriteLine($"Installiere NPM-Paket offline: {pkg.Name} ...");
            Process.Start(new ProcessStartInfo
            {
                FileName = "npm",
                Arguments = $"install -g \"{pkg.Source}\"",
                UseShellExecute = true,
                Verb = "runas"
            })?.WaitForExit();
        }

        static void OfflineUnzip(Package pkg)
        {
            if (!File.Exists(pkg.Source)) return;
            if (string.IsNullOrWhiteSpace(pkg.TargetDir)) return;
            if (Directory.Exists(pkg.TargetDir)) Directory.Delete(pkg.TargetDir, true);
            ZipFile.ExtractToDirectory(pkg.Source, pkg.TargetDir);
            Console.WriteLine($"Entpackt {pkg.Name} nach {pkg.TargetDir}");
        }

        static void OfflineInstallMsi(Package pkg)
        {
            if (!File.Exists(pkg.Source)) return;
            Console.WriteLine($"Installiere MSI-Paket {pkg.Name} ...");
            Process.Start(new ProcessStartInfo
            {
                FileName = pkg.Source,
                UseShellExecute = true,
                Verb = "runas"
            })?.WaitForExit();
        }

        static string CreatePm2StartupScript()
        {
            string scriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "pm2-startup.cmd");
            File.WriteAllText(scriptPath, "pm2 resurrect");
            return scriptPath;
        }

        static void RegisterWindowsService(string serviceName, string executablePath)
        {
            Console.WriteLine($"Registriere Windows-Dienst: {serviceName} ...");
            Process.Start(new ProcessStartInfo
            {
                FileName = "sc.exe",
                Arguments = $"create {serviceName} binPath= \"cmd /c {executablePath}\" start= auto",
                UseShellExecute = true,
                Verb = "runas"
            })?.WaitForExit();
        }

        static void RemoveWindowsService(string serviceName)
        {
            Console.WriteLine($"Entferne Windows-Dienst: {serviceName} ...");
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
