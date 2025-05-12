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

        public static string GetAppCurrentDir(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException(nameof(fileName), "File name cannot be null or empty");
            }

            string currentDirectory = Directory.GetCurrentDirectory();
            string relativePath = Path.Combine(currentDirectory, fileName);
            return relativePath;
        }

        public static string ReadFileFromCurrentDir(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException(nameof(fileName), "File name cannot be null or empty");
            }

            
            string relativePath = GetAppCurrentDir(fileName);

            if (!File.Exists(relativePath))
            {
                throw new FileNotFoundException(relativePath);
            }
            return File.ReadAllText(fileName, Encoding.UTF8);
        }

        public static void WriteFileFromCurrentDir(string fileName, string content)
        {
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException(fileName);
            }

            using (StreamWriter writer = new StreamWriter(GetAppCurrentDir(fileName), append: false, Encoding.UTF8)) { 
                writer.Write(content);
            }
            
        }
    }
}
