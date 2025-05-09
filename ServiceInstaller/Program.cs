using System;
using System.Diagnostics;
using System.IO;
using System.Net.NetworkInformation;
using System.IO.Compression;

namespace ServiceInstaller
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("--- PM2/nginx/MariaDB Setup Tool ---\n");

            if (args.Length == 0)
            {
                Console.WriteLine("Bitte gib 'setup' oder 'remove' als Argument an.");
                return;
            }

            string command = args[0].ToLower();

            if (command == "setup")
            {
                RunSetup();
            }
            else if (command == "remove")
            {
                RunRemoval();
            }
            else
            {
                Console.WriteLine("Unbekanntes Argument. Bitte verwende 'setup' oder 'remove'.");
            }
        }

        static void RunSetup()
        {
            bool online = CheckInternetConnection();

            if (online)
            {
                EnsureChocolateyInstalled();

                InstallPackage("nodejs");
                InstallPackage("nginx");
                InstallPackage("mariadb");

                InstallGlobalNpmPackage("pm2");
            }
            else
            {
                Console.WriteLine("Kein Internetzugang erkannt. Führe Offline-Setup durch...");
                OfflineInstallNode();
                OfflineInstallPM2();
                OfflineInstallNginx();
                OfflineInstallMariaDB();
            }

            string pm2StartScript = CreatePm2StartupScript();
            RegisterWindowsService("PM2AutoStart", pm2StartScript);

            Console.WriteLine("\nSetup abgeschlossen.");
        }

        static void RunRemoval()
        {
            Console.WriteLine("Deinstalliere PM2, nginx und MariaDB ...");
            UninstallPackage("nodejs");
            UninstallPackage("nginx");
            UninstallPackage("mariadb");

            RemoveWindowsService("PM2AutoStart");

            string scriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "pm2-startup.cmd");
            if (File.Exists(scriptPath))
            {
                File.Delete(scriptPath);
                Console.WriteLine("PM2 Startup-Skript gelöscht.");
            }

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
            catch
            {
                return false;
            }
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
            else
            {
                Console.WriteLine("Chocolatey ist bereits installiert.");
            }
        }

        static void InstallPackage(string packageName)
        {
            Console.WriteLine($"Installiere {packageName} ...");
            Process.Start(new ProcessStartInfo
            {
                FileName = "choco",
                Arguments = $"install {packageName} -y",
                UseShellExecute = true,
                Verb = "runas"
            })?.WaitForExit();
        }

        static void UninstallPackage(string packageName)
        {
            Console.WriteLine($"Deinstalliere {packageName} ...");
            Process.Start(new ProcessStartInfo
            {
                FileName = "choco",
                Arguments = $"uninstall {packageName} -y",
                UseShellExecute = true,
                Verb = "runas"
            })?.WaitForExit();
        }

        static void InstallGlobalNpmPackage(string packageName)
        {
            Console.WriteLine($"Installiere NPM-Paket {packageName} ...");
            Process.Start(new ProcessStartInfo
            {
                FileName = "npm",
                Arguments = $"install -g {packageName}",
                UseShellExecute = true,
                Verb = "runas"
            })?.WaitForExit();
        }

        static void OfflineInstallNode()
        {
            Console.WriteLine("Installiere Node.js offline ...");
            string nodeInstaller = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "offline", "node-v20.11.0-x64.msi");
            if (File.Exists(nodeInstaller))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = nodeInstaller,
                    UseShellExecute = true,
                    Verb = "runas"
                })?.WaitForExit();
            }
        }

        static void OfflineInstallPM2()
        {
            Console.WriteLine("Installiere PM2 offline ...");
            string tgzPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "offline", "pm2-5.3.0.tgz");
            if (File.Exists(tgzPath))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "npm",
                    Arguments = $"install -g \"{tgzPath}\"",
                    UseShellExecute = false
                })?.WaitForExit();
            }
        }

        static void OfflineInstallNginx()
        {
            Console.WriteLine("Installiere nginx offline ...");
            string zipPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "offline", "nginx.zip");
            string targetDir = "C:\\nginx";

            if (File.Exists(zipPath))
            {
                if (Directory.Exists(targetDir)) Directory.Delete(targetDir, true);
                ZipFile.ExtractToDirectory(zipPath, targetDir);
                Console.WriteLine("nginx entpackt nach " + targetDir);
            }
        }

        static void OfflineInstallMariaDB()
        {
            Console.WriteLine("Installiere MariaDB offline ...");
            string zipPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "offline", "mariadb.zip");
            string targetDir = "C:\\mariadb";

            if (File.Exists(zipPath))
            {
                if (Directory.Exists(targetDir)) Directory.Delete(targetDir, true);
                ZipFile.ExtractToDirectory(zipPath, targetDir);
                Console.WriteLine("MariaDB entpackt nach " + targetDir);
            }
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