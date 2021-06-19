using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;

using FancyToys.Pages.Dialog;
using System.Diagnostics;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using FancyToys.Pages.Server;
using Windows.UI;
using FancyToys.Pages.Settings;
using FancyToys.Messenger;

namespace FancyToys.Logging
{
    //public enum LogSource
    //{
    //    FancyToys = 0,
    //    FancyServer = 1,
    //}

    public enum LoggingType
    {
        Log = 1,
        Std = 2,
        Dialog = 3,
    }

    public struct LoggingStruct
    {
        public LoggingType type;
        public string content;
    }

    public enum LogType
    {
        Trace = 1,
        Debug = 2,
        Info = 3,
        Warn = 4,
        Error = 5,
        Fatal = 6,
    }
    public struct LogStruct
    {
        public LogType level;
        public string source;
        public string content;
    }
    public enum StdType
    {
        //StandardIn = 0,
        Output = 1,
        Error = 2,
    }
    public struct StdStruct
    {
        public StdType type;
        public string process;
        public string content;
    }

    struct DialogStruct
    {
        public string title;
        public string content;
    }

    class LoggingManager
    {
        private static char sep = '>';
        private static Queue<LogCacheStruct> cacheQueue = new Queue<LogCacheStruct>();
        private struct LogCacheStruct
        {
            public string source;
            public string content;
            public Color color;
        }
        public static readonly Dictionary<LogType, Color> LogForegroundColors = new Dictionary<LogType, Color>()
        {
            { LogType.Trace, Colors.Gray },
            { LogType.Debug, Colors.Cyan },
            { LogType.Info, Colors.MediumSpringGreen },
            { LogType.Warn, Colors.Yellow },
            { LogType.Error, Colors.DeepPink },
            { LogType.Fatal, Colors.Red }
        };
        public static readonly Dictionary<StdType, Color> StdForegroundColors = new Dictionary<StdType, Color>()
        {
            { StdType.Output, Colors.Aquamarine },
            { StdType.Error, Colors.Firebrick },
        };


        LoggingManager() { }

        public static void Deal(string message)
        {
            LoggingStruct ls = JsonConvert.DeserializeObject<LoggingStruct>(message);

            switch (ls.type)
            {
                case LoggingType.Log:
                    DealWithLog(ls.content);
                    break;
                case LoggingType.Std:
                    DealWithStd(ls.content);
                    break;
                case LoggingType.Dialog:
                    DealWithDialog(ls.content);
                    break;
                default:
                    Warn("Invalid logging type");
                    break;
            }
        }

        private static void DealWithLog(string content)
        {
            bool success = MessageManager.ParseStruct<LogStruct>(content, out LogStruct sdu);
            if (success)
            {
                switch (sdu.level)
                {
                    case LogType.Trace:
                        PrintLog(LogType.Trace, $"FancyServer.{sdu.source}", sdu.content);
                        break;
                    case LogType.Debug:
                        PrintLog(LogType.Debug, $"FancyServer.{sdu.source}", sdu.content);
                        break;
                    case LogType.Info:
                        PrintLog(LogType.Info, $"FancyServer.{sdu.source}", sdu.content);
                        break;
                    case LogType.Warn:
                        PrintLog(LogType.Warn, $"FancyServer.{sdu.source}", sdu.content);
                        break;
                    case LogType.Error:
                        PrintLog(LogType.Error, $"FancyServer.{sdu.source}", sdu.content);
                        break;
                    case LogType.Fatal:
                        PrintLog(LogType.Fatal, $"FancyServer.{sdu.source}", sdu.content);
                        break;
                }
            }
        }

        private static void DealWithStd(string content)
        {
            bool success = MessageManager.ParseStruct<StdStruct>(content, out StdStruct sdu);
            if (success && sdu.content.Trim().Length != 0)
            {
                switch(sdu.type)
                {
                    case StdType.Output:
                        if (SettingsClerk.Clerk.STStdLevel == StdType.Output)
                        {
                            Print(sdu.process, sdu.content, StdForegroundColors[StdType.Output]);
                        }
                        break;
                    case StdType.Error:
                        Print(sdu.process, sdu.content, StdForegroundColors[StdType.Error]);
                        break;
                }
            }
        }

        private async static void DealWithDialog(string content)
        {
            bool success = MessageManager.ParseStruct<DialogStruct>(content, out DialogStruct sdu);
            if (success)
            {
                await CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    await MessageDialog.Info(sdu.title, sdu.content);
                });
            }
        }


        public static void Trace(string msg, int depth = 1)
        {
            PrintLog(LogType.Trace, CallerName(depth + 1), msg);
        }
        public static void Debug(string msg, int depth = 1)
        {
            PrintLog(LogType.Debug, CallerName(depth + 1), msg);
        }
        public static void Info(string msg, int depth = 1)
        {
            PrintLog(LogType.Info, CallerName(depth + 1), msg);
        }
        public static void Warn(string msg, int depth = 1)
        {
            PrintLog(LogType.Warn, CallerName(depth + 1), msg);
        }
        public static void Error(string msg, int depth = 1)
        {
            PrintLog(LogType.Error, CallerName(depth + 1), msg);
        }
        public static void Fatal(string msg, int depth = 1)
        {
            PrintLog(LogType.Fatal, CallerName(depth + 1), msg);
        }

        private static void PrintLog(LogType level, string source, string content)
        {
            if (level >= SettingsClerk.Clerk.STLogLevel)
            {
                Print($"{source}{sep} ", content, LogForegroundColors[level]);
            }
        }

        private static void Print(string src, string cnt, Color clr)
        {
            ServerPage page = ServerPage.Page;
            if (page == null)
            {
                cacheQueue.Enqueue(new LogCacheStruct
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
                LogCacheStruct lc = cacheQueue.Dequeue();
                _ = CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    ServerPage.Page.PrintLog(lc.source, lc.content, lc.color);
                });
            }
        }

        private static string CallerName(int depth)
        {
            MethodBase method = new StackTrace().GetFrame(depth).GetMethod();
            return $"FancyToys.{method.ReflectedType.Name}.{method.Name}";
        }
    }
}
