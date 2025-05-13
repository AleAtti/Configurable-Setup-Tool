using System;
using SetupTool.Helper;
using SetupTool.model;

namespace SetupTool.Services
{
    public static class ConfigurationService
    {
        /// <summary>
        /// Loads a configuration object of type T from the specified file path.
        /// </summary>
        /// <typeparam name="T">The type of the configuration object.</typeparam>
        /// <param name="filePath">The path to the configuration file.</param>
        /// <returns>The deserialized configuration object.</returns>
        public static T LoadConfiguration<T>(string filePath)
        {
            try
            {
                Console.WriteLine($"Loading configuration from: {filePath}");
                string json = FileHelper.ReadFileFromCurrentDir(filePath);
                return JsonModel<T>.FromJsonString(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading configuration: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Saves a configuration object of type T to the specified file path.
        /// </summary>
        /// <typeparam name="T">The type of the configuration object.</typeparam>
        /// <param name="config">The configuration object to save.</param>
        /// <param name="filePath">The path to the configuration file.</param>
        public static void SaveConfiguration<T>(T config, string filePath)
        {
            try
            {
                Console.WriteLine($"Saving configuration to: {filePath}");
                JsonModel<T>.ToJsonFile(config, filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving configuration: {ex.Message}");
                throw;
            }
        }
    }
}
