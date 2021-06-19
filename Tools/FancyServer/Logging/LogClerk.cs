using System.Diagnostics;
using System.Reflection;

using NLog;

namespace FancyServer.Logging
{
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

    class LogClerk
    {
        private static readonly Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public static LogType LogLevel { get => logLevel; set => logLevel = value; }
        private static LogType logLevel = LogType.Trace;


        public static void Trace(string msg, int depth = 1)
        {
            logger.Trace(msg);
            Send(LogType.Trace, depth + 1, msg);
        }

        public static void Debug(string msg, int depth = 1)
        {
            logger.Debug(msg);
            Send(LogType.Debug, depth + 1, msg);
        }

        public static void Info(string msg, int depth = 1)
        {
            logger.Info(msg);
            Send(LogType.Info, depth + 1, msg);
        }

        public static void Warn(string msg, int depth = 1)
        {
            logger.Warn(msg);
            Send(LogType.Warn, depth + 1, msg);
        }

        public static void Error(string msg, int depth = 1)
        {
            logger.Error(msg);
            Send(LogType.Error, depth + 1, msg);
        }

        public static void Fatal(string msg, int depth = 1)
        {
            logger.Fatal(msg);
            Send(LogType.Fatal, depth + 1, msg);
        }

        private static void Send(LogType type, int depth, string content)
        {
            if (type >= LogLevel)
            {
                LoggingManager.Send(new LogStruct
                {
                    level = type,
                    source = CallerName(depth+1),
                    content = content
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
