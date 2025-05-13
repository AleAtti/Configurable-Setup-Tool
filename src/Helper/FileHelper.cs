using System;
using System.IO;

namespace SetupTool.Helper
{
    public static class FileHelper
    {
        /// <summary>
        /// Reads the content of a file from the current directory.
        /// </summary>
        /// <param name="fileName">The name of the file to read.</param>
        /// <returns>The content of the file as a string.</returns>
        public static string ReadFileFromCurrentDir(string fileName)
        {
            try
            {
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
                if (!File.Exists(filePath))
                    throw new FileNotFoundException($"File not found: {filePath}");

                return File.ReadAllText(filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading file '{fileName}': {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Writes content to a file in the current directory.
        /// </summary>
        /// <param name="fileName">The name of the file to write to.</param>
        /// <param name="content">The content to write to the file.</param>
        public static void WriteFileToCurrentDir(string fileName, string content)
        {
            try
            {
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
                File.WriteAllText(filePath, content);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing to file '{fileName}': {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Checks if a file exists in the current directory.
        /// </summary>
        /// <param name="fileName">The name of the file to check.</param>
        /// <returns>True if the file exists, otherwise false.</returns>
        public static bool FileExistsInCurrentDir(string fileName)
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
            return File.Exists(filePath);
        }
    }
}
