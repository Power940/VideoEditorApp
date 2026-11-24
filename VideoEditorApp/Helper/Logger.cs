using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace VideoEditorApp.Helper
{
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
