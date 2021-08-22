using System.Reflection;
using System.Diagnostics;

using NLog;


namespace FancyServer.Logging {

    public enum LogType {
        Trace = 1,
        Debug = 2,
        Info = 3,
        Warn = 4,
        Error = 5,
        Fatal = 6,
    }

    public struct LogStruct {
        public LogType level;
        public string source;
        public string content;
    }

    internal static class LogClerk {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public static LogType LogLevel { get; set; } = LogType.Trace;


        public static void Trace(string msg, int depth = 1) {
            Logger.Trace(msg);
            Send(LogType.Trace, depth + 1, msg);
        }

        public static void Debug(string msg, int depth = 1) {
            Logger.Debug(msg);
            Send(LogType.Debug, depth + 1, msg);
        }

        public static void Info(string msg, int depth = 1) {
            Logger.Info(msg);
            Send(LogType.Info, depth + 1, msg);
        }

        public static void Warn(string msg, int depth = 1) {
            Logger.Warn(msg);
            Send(LogType.Warn, depth + 1, msg);
        }

        public static void Error(string msg, int depth = 1) {
            Logger.Error(msg);
            Send(LogType.Error, depth + 1, msg);
        }

        public static void Fatal(string msg, int depth = 1) {
            Logger.Fatal(msg);
            Send(LogType.Fatal, depth + 1, msg);
        }

        private static void Send(LogType type, int depth, string content) {
            if (type >= LogLevel) {
                LoggingManager.Send(new LogStruct {
                    level = type,
                    source = CallerName(depth + 1),
                    content = content
                });
            }
        }

        private static string CallerName(int depth) {
            MethodBase method = new StackTrace().GetFrame(depth).GetMethod();
            return $"{method?.ReflectedType?.Name}.{method?.Name}";
        }
    }

}