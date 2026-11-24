using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace VideoEditorApp.Helper
{
    class Logger
    {
        ConcurrentQueue<string> logs = [];
    }
}
