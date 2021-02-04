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
        public string content;
    }
    class LoggingManager
    {
        public static bool consoleDebug = true;
        public static LogLevel LoggingLevel = LogLevel.Trace;
        private static readonly Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static void Dialog(string message, LogLevel level=LogLevel.Info)
        {
            Send(LogType.Dialog, level, message);
        }

        public static void Trace(string msg, int depth = 1)
        {
            string message = CallerName(depth + 1) + msg;
            logger.Trace(message);
            Send(LogType.Normal, LogLevel.Trace, message);
        }

        public static void Debug(string msg, int depth = 1)
        {
            string message = CallerName(depth + 1) + msg;
            logger.Debug(message);
            Send(LogType.Normal, LogLevel.Debug, message);
        }

        public static void Info(string msg, int depth = 1)
        {
            string message = CallerName(depth + 1) + msg;
            logger.Info(message);
            Send(LogType.Normal, LogLevel.Info, message);
        }

        public static void Warn(string msg, int depth = 1)
        {
            string message = CallerName(depth + 1) + msg;
            logger.Warn(message);
            Send(LogType.Normal, LogLevel.Warn, message);
        }

        public static void Error(string msg, int depth = 1)
        {
            string message = CallerName(depth + 1) + msg;
            logger.Error(message);
            Send(LogType.Normal, LogLevel.Error, message);
        }

        public static void Fatal(string msg, int depth = 1)
        {
            string message = CallerName(depth + 1) + msg;
            logger.Fatal(message);
            Send(LogType.Normal, LogLevel.Fatal, message);
        }

        private static void Send(LogType lt, LogLevel ll, string message)
        {
            if (ll >= LoggingLevel)
            {
                MessageManager.Send(PDU(lt, ll, message));
            }
        }

        private static LoggingStruct PDU(LogType lt, LogLevel ll, string message)
        {
            LoggingStruct pdu = new LoggingStruct
            {
                type = lt,
                level = ll,
                content = message
            };
            return pdu;
        }

        private static string CallerName(int depth)
        {
            MethodBase method = new StackTrace().GetFrame(depth).GetMethod();
            return $"[{method.ReflectedType.Name}.{method.Name}] ";
        }
    }
}
