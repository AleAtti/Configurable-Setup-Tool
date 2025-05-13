using System;
using SetupTool.model;
using SetupTool.Core;
using SetupTool.Helper;
using SetupTool.Services;

namespace SetupTool
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("--- Configurable Setup Tool ---\n");

            // Load configuration file path from settings
            string configPath = ServiceInstaller.Properties.Settings.Default.PackageConfigPath;

            // Load the package manifest
            PackageManifest manifest;
            try
            {
                manifest = ConfigurationService.LoadConfiguration<PackageManifest>(configPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading configuration: {ex.Message}");
                return;
            }

            // Validate command-line arguments
            if (args.Length == 0)
            {
                Console.WriteLine("Please specify 'setup' or 'remove' as an argument.");
                return;
            }

            string command = args[0].ToLower();

            // Execute the appropriate operation
            if (command == "setup")
            {
                var setup = new Setup(manifest);
                setup.RunSetup();
            }
            else if (command == "remove")
            {
                var removal = new Removal(manifest);
                removal.RunRemoval();
            }
            else
            {
                Console.WriteLine("Unknown argument. Please use 'setup' or 'remove'.");
            }
        }
    }
}
