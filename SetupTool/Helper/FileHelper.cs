using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceInstaller.Helper
{
    public class FileHelper
    {
        public static string GetFileName(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path), "Path cannot be null or empty");
            }
            return Path.GetFileName(path);
        }
        public static string GetDirectoryName(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path), "Path cannot be null or empty");
            }
            return Path.GetDirectoryName(path);
        }

        public static string GetRelativPath(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException(nameof(fileName), "File name cannot be null or empty");
            }
            string currentDirectory = Directory.GetCurrentDirectory();
            string relativePath = Path.Combine(currentDirectory, fileName);
            return relativePath;
        }
    }
}
