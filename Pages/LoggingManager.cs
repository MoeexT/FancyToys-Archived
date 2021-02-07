using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using FancyToys.Pages.Dialog;
using FancyToys.Pages.Nursery;
using System.Diagnostics;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using FancyToys.Pages.Server;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.Storage;
using FancyToys.Pages.Settings;

namespace FancyToys.Pages
{
    public enum LogSource
    {
        FancyToys = 0,
        FancyServer = 1,
        Process = 2,
    }

    public enum LogType
    {
        Normal = 1,
        Dialog = 2,
    }

    public enum LogLevel
    {
        Trace = 1,
        Debug = 2,
        Info = 3,
        Warn = 4,
        Error = 5,
        Fatal = 6,

    }
    public struct LoggingStruct
    {
        public LogType type;
        public LogLevel level;
        public string source;
        public string content;
    }

    class LoggingManager
    {
        private struct LogCache
        {
            public string source;
            public string content;
            public Color color;
        }
        private static Queue<LogCache> cacheQueue = new Queue<LogCache>();
        public static readonly Dictionary<LogLevel, Color> LogForegroundColors = new Dictionary<LogLevel, Color>()
        {
            { LogLevel.Trace, Colors.Gray },
            { LogLevel.Debug, Colors.Cyan },
            { LogLevel.Info, Colors.MediumSpringGreen },
            { LogLevel.Warn, Colors.Yellow },
            { LogLevel.Error, Colors.DeepPink },
            { LogLevel.Fatal, Colors.Red }
        };
        public static readonly Dictionary<StandardFileType, Color> StdForegroundColors = new Dictionary<StandardFileType, Color>()
        {
            { StandardFileType.Output, Colors.Aquamarine },
            { StandardFileType.Error, Colors.Firebrick },
        };

        LoggingManager()
        {
            
        }

        public static void Deal(string message)
        {
            LoggingStruct ls = JsonConvert.DeserializeObject<LoggingStruct>(message);
            if (ls.type == LogType.Dialog)
            {
                Dialog(ls.content, LogSource.FancyServer);
            }
            else
            {
                PrintLog(ls.level, $"{LogSource.FancyServer}.{ls.source}", ls.content);
            }
        }

        private static async void Dialog(string message, LogSource source=LogSource.FancyToys)
        {
            await CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
             {
                 await MessageDialog.Info(source.ToString(), message);
             });
        }

        public static void StandardOutput(string processName, string msg)
        {
            if (SettingsClerk.Clerk.STStdLevel == StandardFileType.Output)
            {
                Print(processName, msg, StdForegroundColors[StandardFileType.Output]);
            }
        }
        
        public static void StandardError(string processName, string msg)
        {
            Print(processName, msg, StdForegroundColors[StandardFileType.Error]);
        }

        public static void Trace(string msg, int depth = 1, LogSource source=LogSource.FancyToys)
        {
            PrintLog(LogLevel.Trace, $"{source}.{CallerName(depth + 1)}", msg);
        }
        public static void Info(string msg, int depth = 1, LogSource source = LogSource.FancyToys)
        {
            PrintLog(LogLevel.Info, $"{source}.{CallerName(depth + 1)}", msg);
        }
        public static void Debug(string msg, int depth = 1, LogSource source = LogSource.FancyToys)
        {
            PrintLog(LogLevel.Debug, $"{source}.{CallerName(depth + 1)}", msg);
        }
        public static void Warn(string msg, int depth = 1, LogSource source = LogSource.FancyToys)
        {
            PrintLog(LogLevel.Warn, $"{source}.{CallerName(depth + 1)}", msg);
        }
        public static void Error(string msg, int depth = 1, LogSource source = LogSource.FancyToys)
        {
            PrintLog(LogLevel.Error, $"{source}.{CallerName(depth + 1)}", msg);
        }
        public static void Fatal(string msg, int depth = 1, LogSource source = LogSource.FancyToys)
        {
            PrintLog(LogLevel.Fatal, $"{source}.{CallerName(depth + 1)}", msg);
        }

        private static void PrintLog(LogLevel level, string source, string content)
        {
            if (level >= SettingsClerk.Clerk.STLogLevel)
            {
                Print(source, content, LogForegroundColors[level]);
            }
        }

        private static void Print(string src, string cnt, Color clr)
        {
            ServerPage page = ServerPage.Page;
            if (page == null)
            {
                cacheQueue.Enqueue(new LogCache
                {
                    source = src,
                    content = cnt,
                    color = clr,
                });
            }
            else
            {
                _ = CoreApplication.MainView.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal, () =>
                {
                    page.PrintLog(src, cnt, clr);
                });
            }
        }

        public static void FlushLogCache()
        {
            while (cacheQueue.Count > 0)
            {
                LogCache lc = cacheQueue.Dequeue();
                _ = CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    ServerPage.Page.PrintLog(lc.source, lc.content, lc.color);
                });
            }
        }

        private static string CallerName(int depth)
        {
            MethodBase method = new StackTrace().GetFrame(depth).GetMethod();
            return $"{method.ReflectedType.Name}.{method.Name}";
        }
    }
}
