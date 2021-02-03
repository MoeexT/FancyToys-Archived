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
        Trace = 1,
        Debug = 2,
        Info = 3,
        Warn = 4,
        Error = 5,
        Fatal = 6,
        Dialog = 7,     // 用于在前端的提示

    }
    public struct LoggingStruct
    {
        public LogType type;
        public string content;
    }

    class LoggingManager
    {
        private static Queue<LoggingStruct> logCache = new Queue<LoggingStruct>();
        private static readonly Dictionary<LogType, Color> logColor = new Dictionary<LogType, Color>()
        {
            { LogType.Trace, Colors.LightGray },
            { LogType.Debug, Colors.LightBlue },
            { LogType.Info, Colors.LightGreen },
            { LogType.Warn, Colors.Yellow },
            { LogType.Error, Colors.PaleVioletRed},
            { LogType.Fatal, Colors.DarkRed}
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
                PrintToPage(LogSource.FancyServer, ls.type, ls.content);
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
                PrintToPage(source, LogType.Trace, CallerName(depth + 1) + msg);
            }
            else
            {
                PrintToPage(source, LogType.Trace, msg);
            }
        }

        public static void Info(string msg, int depth = 1, LogSource source = LogSource.FancyToys)
        {
            if (source == LogSource.FancyToys)
            {
                PrintToPage(source, LogType.Info, CallerName(depth + 1) + msg);
            }
            else
            {
                PrintToPage(source, LogType.Info, msg);
            }
        }

        public static void Debug(string msg, int depth = 1, LogSource source = LogSource.FancyToys)
        {
            if (source == LogSource.FancyToys)
            {
                PrintToPage(source, LogType.Debug, CallerName(depth + 1) + msg);
            }
            else
            {
                PrintToPage(source, LogType.Debug, msg);
            }
        }

        public static void Warn(string msg, int depth = 1, LogSource source = LogSource.FancyToys)
        {
            if (source == LogSource.FancyToys)
            {
                PrintToPage(source, LogType.Warn, CallerName(depth + 1) + msg);
            }
            else
            {
                PrintToPage(source, LogType.Warn, msg);
            }
        }

        public static void Error(string msg, int depth = 1, LogSource source = LogSource.FancyToys)
        {
            if (source == LogSource.FancyToys)
            {
                PrintToPage(source, LogType.Error, CallerName(depth + 1) + msg);
            }
            else
            {
                PrintToPage(source, LogType.Error, msg);
            }
        }

        public static void Fatal(string msg, int depth = 1, LogSource source = LogSource.FancyToys)
        {
            if (source == LogSource.FancyToys)
            {
                PrintToPage(source, LogType.Fatal, CallerName(depth + 1) + msg);
            }
            else
            {
                PrintToPage(source, LogType.Fatal, msg);
            }
        }

        private static void PrintToPage(LogSource source, LogType type, string message)
        {
            if (type >= SettingsClerk.Clerk.STLogLevel)
            {
                ServerPage page = ServerPage.Page;
                if (page == null)
                {
                    logCache.Enqueue(new LoggingStruct
                    {
                        type = type,
                        content = message
                    });
                }
                else
                {
                    _ = CoreApplication.MainView.Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal, () =>
                    {
                        page.PrintLog(logColor[type], message);
                    });
                }
            }
        }

        public static void FlushLogCache()
        {
            while (logCache.Count > 0)
            {
                LoggingStruct ls = logCache.Dequeue();
                if (ls.type >= SettingsClerk.Clerk.STLogLevel)
                {
                    _ = CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                     {
                         ServerPage.Page.PrintLog(logColor[ls.type], ls.content);
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
