using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace VideoEditorApp.Helper
{
    static class Logger
    {
        private static readonly ConcurrentQueue<string> logs = [];
        private static readonly Thread logWriteThread = new(LoggingLoop);
        private static readonly string logFilePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        private static readonly StreamWriter logWriter;


        static Logger()
        {
            logWriteThread.IsBackground = true;
            logWriter = File.CreateText(Path.Combine(logFilePath, "Logs", DateTime.Now.ToString()));
        }

        static private void LoggingLoop()
        {
            while (true)
            {
                if (logs.IsEmpty)
                {
                    Thread.Sleep(50);
                    continue;
                }

                while (!logs.IsEmpty)
                {
                    if (logs.TryDequeue(out string? result) && !String.IsNullOrWhiteSpace(result))
                    {
                        logWriter.WriteLine(result);
                    }
                }

                Thread.Sleep(50);
            }
        }
    class Logger
    {
        ConcurrentQueue<string> logs = [];

        public void Log(string message)
        {
            string currentTime = DateTime.Now.ToString();
            logs.Enqueue($"({currentTime}) Message: {message}");
        }
    }
}
