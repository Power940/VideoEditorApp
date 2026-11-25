using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;

namespace VideoEditorApp.Helper
{
    static class Logger
    {
        private static readonly BlockingCollection<string> logs = [];
        private static readonly Thread logWriteThread;
        private static readonly string logDirPath = Path.Combine(FileSystem.Current.AppDataDirectory, "Logs");
        private static readonly string logFilePath;


        static Logger()
        {
            Directory.CreateDirectory(logDirPath);
            logFilePath = Path.Combine(logDirPath, DateTime.Now.ToString("MM-dd_HH-mm-ss") + ".txt");

            logWriteThread = new Thread(LoggingLoop)
            {
                IsBackground = true,
                Name = "FileLoggerThread"
            };
            logWriteThread.Start();
        }

        private static void LoggingLoop()
        {
            try
            {
                using StreamWriter writer = new(logFilePath, append: true) { AutoFlush = true };
                foreach (string message in logs.GetConsumingEnumerable())
                {
                    if (!string.IsNullOrWhiteSpace(message))
                    {
                        writer.WriteLine(message);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Logger failed: {ex.Message}");
            }
        }

        public static void Log(string message)
        {
            string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
            logs.Add($"[{currentTime}] | {message}");
        }
    }
}
