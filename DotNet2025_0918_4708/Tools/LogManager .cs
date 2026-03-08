using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools
{
    public sealed class LogManager
    {
        private static string logFolderName = "Log";
        private static readonly LogManager instance = new LogManager();

        public static LogManager Instance
        {
            get { return instance; }
        }
        public static string getCurFolderPath()
        {
            // מחזיר את הנתיב של תיקיית Log בתיקיית העבודה הנוכחית
            return System.IO.Path.Combine(Environment.CurrentDirectory, logFolderName);
        }

        public static string getCurFilePath()
        {
            // מחזיר נתיב לקובץ לוג חדש בתיקיית החודש הנוכחי
            string monthDir = DateTime.Now.ToString("yyyy-MM");
            string logDir = System.IO.Path.Combine(getCurFolderPath(), monthDir);
            string fileName = $"Log_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
            return System.IO.Path.Combine(logDir, fileName);
        }

        public static void Log(string projectName, string funcName, string message)
        {
            try
            {
                // יצירת תיקיית Log אם לא קיימת
                string baseLogDir = getCurFolderPath();
                if (!System.IO.Directory.Exists(baseLogDir))
                {
                    System.IO.Directory.CreateDirectory(baseLogDir);
                }

                // יצירת תת-תיקייה לפי שנה-חודש
                string monthDir = DateTime.Now.ToString("yyyy-MM");
                string monthLogDir = System.IO.Path.Combine(baseLogDir, monthDir);
                if (!System.IO.Directory.Exists(monthLogDir))
                {
                    System.IO.Directory.CreateDirectory(monthLogDir);
                }

                // שם קובץ הלוג
                string fileName = $"Log_{DateTime.Now:yyyyMMdd}.txt";
                string filePath = System.IO.Path.Combine(monthLogDir, fileName);

                // כתיבת הלוג
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
                string baseLogDir = getCurFolderPath();
                if (System.IO.Directory.Exists(baseLogDir))
                {
                    int deletedCount = 0;
                    // עובר על כל תתי התיקיות (חודשים)
                    foreach (var monthDir in System.IO.Directory.GetDirectories(baseLogDir))
                    {
                        var logFiles = System.IO.Directory.GetFiles(monthDir, "Log_*.txt");
                        foreach (var file in logFiles)
                        {
                            if (System.IO.File.GetCreationTime(file) < DateTime.Now.AddMonths(-2))
                            {
                                System.IO.File.Delete(file);
                                deletedCount++;
                            }
                        }
                    }
                    return $"Deleted {deletedCount} log files.";
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
