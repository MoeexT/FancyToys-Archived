using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FancyServer.Messenger
{
    enum LogType
    {
        Normal = 1,
        Dialog = 2,
    }
    enum LogLevel
    {
        Trace = 1,
        Debug = 2,
        Info = 3,
        Warn = 4,
        Error = 5,
        Fatal = 6,

    }
    struct LoggingStruct
    {
        public LogType type;
        public LogLevel level;
        public string source;
        public string content;
    }
    class LoggingManager
    {
        public static bool consoleDebug = true;
        public static LogLevel LoggingLevel = LogLevel.Trace;
        private static readonly Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static void Dialog(string message, LogLevel level=LogLevel.Info)
        {
            Send(LogType.Dialog, level, 2, message);
        }

        public static void Trace(string msg, int depth = 1)
        {
            logger.Trace(msg);
            Send(LogType.Normal, LogLevel.Trace, depth + 1, msg);
        }

        public static void Debug(string msg, int depth = 1)
        {
            logger.Debug(msg);
            Send(LogType.Normal, LogLevel.Debug, depth + 1, msg);
        }

        public static void Info(string msg, int depth = 1)
        {
            logger.Info(msg);
            Send(LogType.Normal, LogLevel.Info, depth + 1, msg);
        }

        public static void Warn(string msg, int depth = 1)
        {
            logger.Warn(msg);
            Send(LogType.Normal, LogLevel.Warn, depth + 1, msg);
        }

        public static void Error(string msg, int depth = 1)
        {
            logger.Error(msg);
            Send(LogType.Normal, LogLevel.Error, depth + 1, msg);
        }

        public static void Fatal(string msg, int depth = 1)
        {
            logger.Fatal(msg);
            Send(LogType.Normal, LogLevel.Fatal, depth + 1, msg);
        }

        private static void Send(LogType lt, LogLevel ll, int depth, string message)
        {
            if (ll >= LoggingLevel)
            {
                MessageManager.Send(PDU(lt, ll, CallerName(depth+1), message));
            }
        }

        private static LoggingStruct PDU(LogType lt, LogLevel ll, string source, string content)
        {
            LoggingStruct pdu = new LoggingStruct
            {
                type = lt,
                level = ll,
                source = source,
                content = content,
            };
            return pdu;
        }

        private static string CallerName(int depth)
        {
            MethodBase method = new StackTrace().GetFrame(depth).GetMethod();
            return $"{method.ReflectedType.Name}.{method.Name}";
        }
    }
}
