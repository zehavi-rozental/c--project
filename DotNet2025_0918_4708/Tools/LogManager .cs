using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Tools
{
    public static class LogManager
    {
        private static string logFilePath = "Log";
        public static string getCurFolderPath()
        {
            return Environment.CurrentDirectory;
        }
        public static string getCurFilePath()
        {
            string fileName = $"Log_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
            return System.IO.Path.Combine(Environment.CurrentDirectory, fileName);
        }
        public static void Log(string projectName, string funcName, string message)
        {
            try
            {
                string logDir = getCurFolderPath();
                if (!System.IO.Directory.Exists(logDir))
                {
                    System.IO.Directory.CreateDirectory(logDir);
                }
                string fileName = $"Log_{DateTime.Now:yyyyMMdd}.txt";
                string filePath = System.IO.Path.Combine(logDir, fileName);
                string logMessage = $"{DateTime.Now}\t{projectName}.{funcName}:\t{message}";
                System.IO.File.AppendAllText(filePath, logMessage + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing log: {ex.Message}");
            }
        }
        public static string CleanLogDirectory()
        {
            try
            {
                string logDir = getCurFolderPath();
                if (System.IO.Directory.Exists(logDir))
                {
                    var logFiles = System.IO.Directory.GetFiles(logDir, "Log_*.txt");
                    foreach (var file in logFiles)
                    {
                        if (System.IO.File.GetCreationTime(file) < DateTime.Now.AddMonths(-2))
                        System.IO.File.Delete(file);
                    }
                    return $"Deleted {logFiles.Length} log files.";
                }
                else
                {
                    return "Log directory does not exist.";
                }
            }
            catch (Exception ex)
            {
                return $"Error cleaning logs: {ex.Message}";
            }
        }

    }
}
