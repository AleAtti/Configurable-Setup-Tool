using System;
using System.Diagnostics;

namespace SetupTool.Helper
{
    public static class ProcessHelper
    {
        /// <summary>
        /// Starts a process with the specified arguments and waits for it to exit.
        /// </summary>
        /// <param name="fileName">The name of the executable file to start.</param>
        /// <param name="arguments">The arguments to pass to the process.</param>
        /// <param name="useShellExecute">Whether to use the operating system shell to start the process.</param>
        /// <param name="runAsAdmin">Whether to run the process with elevated privileges.</param>
        public static void StartProcess(string fileName, string arguments, bool useShellExecute = true, bool runAsAdmin = false)
        {
            try
            {
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = arguments,
                    UseShellExecute = useShellExecute,
                    Verb = runAsAdmin ? "runas" : string.Empty
                };

                using (var process = Process.Start(processStartInfo))
                {
                    process?.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error starting process '{fileName}': {ex.Message}");
            }
        }
    }
}
