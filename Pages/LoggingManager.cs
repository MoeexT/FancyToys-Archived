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
        public string content;
    }

    class LoggingManager
    {
        private static Queue<LoggingStruct> logCache = new Queue<LoggingStruct>();
        public static readonly Dictionary<LogLevel, Color> LogForegroundColors = new Dictionary<LogLevel, Color>()
        {
            { LogLevel.Trace, Colors.Gray },
            { LogLevel.Debug, Colors.Cyan },
            { LogLevel.Info, Colors.MediumSpringGreen },
            { LogLevel.Warn, Colors.Yellow },
            { LogLevel.Error, Colors.DeepPink},
            { LogLevel.Fatal, Colors.Red}
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
                PrintToPage(LogSource.FancyServer, ls.level, ls.content);
            }
        }

        private static async void Dialog(string message, LogSource source=LogSource.FancyToys)
        {
            await CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
             {
                 await MessageDialog.Info(source.ToString(), message);
             });
        }

        public static void Trace(string msg, int depth = 1, LogSource source=LogSource.FancyToys)
        {
            if (source == LogSource.FancyToys)
            {
                PrintToPage(source, LogLevel.Trace, CallerName(depth + 1) + msg);
            }
            else
            {
                PrintToPage(source, LogLevel.Trace, msg);
            }
        }

        public static void Info(string msg, int depth = 1, LogSource source = LogSource.FancyToys)
        {
            if (source == LogSource.FancyToys)
            {
                PrintToPage(source, LogLevel.Info, CallerName(depth + 1) + msg);
            }
            else
            {
                PrintToPage(source, LogLevel.Info, msg);
            }
        }

        public static void Debug(string msg, int depth = 1, LogSource source = LogSource.FancyToys)
        {
            if (source == LogSource.FancyToys)
            {
                PrintToPage(source, LogLevel.Debug, CallerName(depth + 1) + msg);
            }
            else
            {
                PrintToPage(source, LogLevel.Debug, msg);
            }
        }

        public static void Warn(string msg, int depth = 1, LogSource source = LogSource.FancyToys)
        {
            if (source == LogSource.FancyToys)
            {
                PrintToPage(source, LogLevel.Warn, CallerName(depth + 1) + msg);
            }
            else
            {
                PrintToPage(source, LogLevel.Warn, msg);
            }
        }

        public static void Error(string msg, int depth = 1, LogSource source = LogSource.FancyToys)
        {
            if (source == LogSource.FancyToys)
            {
                PrintToPage(source, LogLevel.Error, CallerName(depth + 1) + msg);
            }
            else
            {
                PrintToPage(source, LogLevel.Error, msg);
            }
        }

        public static void Fatal(string msg, int depth = 1, LogSource source = LogSource.FancyToys)
        {
            if (source == LogSource.FancyToys)
            {
                PrintToPage(source, LogLevel.Fatal, CallerName(depth + 1) + msg);
            }
            else
            {
                PrintToPage(source, LogLevel.Fatal, msg);
            }
        }

        private static void PrintToPage(LogSource source, LogLevel level, string message)
        {
            if (level >= SettingsClerk.Clerk.STLogLevel)
            {
                ServerPage page = ServerPage.Page;
                if (page == null)
                {
                    logCache.Enqueue(new LoggingStruct
                    {
                        level = level,
                        content = message
                    });
                }
                else
                {
                    _ = CoreApplication.MainView.Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal, () =>
                    {
                        page.PrintLog(message, LogForegroundColors[level]);
                    });
                }
            }
        }

        public static void FlushLogCache()
        {
            while (logCache.Count > 0)
            {
                LoggingStruct ls = logCache.Dequeue();
                if (ls.level >= SettingsClerk.Clerk.STLogLevel)
                {
                    _ = CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                     {
                         ServerPage.Page.PrintLog(ls.content, LogForegroundColors[ls.level]);
                     });
                }
            }
        }

        private static string CallerName(int depth)
        {
            MethodBase method = new StackTrace().GetFrame(depth).GetMethod();
            return $"[{method.ReflectedType.Name}.{method.Name}] ";
        }
    }
}
