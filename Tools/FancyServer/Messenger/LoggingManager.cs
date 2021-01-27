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
        Trace = 1,
        Debug = 2,
        Info = 3,
        Warn = 4,
        Error = 5,
        Fatal = 6,
        Dialog = 7,     // 用于在前端的提示

    }
    struct LoggingStruct
    {
        public LogType type;
        public string content;
    }
    class LoggingManager
    {
        public static bool consoleDebug = true;
        public static int LoggingLevel = 1;
        private static readonly Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static void Dialog(string message)
        {
            Send(LogType.Dialog, message);
        }

        public static void Trace(string msg, int depth = 1)
        {
            string message = CallerName(depth + 1) + msg;
            logger.Trace(message);
            Send(LogType.Trace, message);
        }

        public static void Debug(string msg, int depth = 1)
        {
            string message = CallerName(depth + 1) + msg;
            logger.Debug(message);
            Send(LogType.Debug, message);
        }

        public static void Info(string msg, int depth = 1)
        {
            string message = CallerName(depth + 1) + msg;
            logger.Info(message);
            Send(LogType.Info, message);
        }

        public static void Warn(string msg, int depth = 1)
        {
            string message = CallerName(depth + 1) + msg;
            logger.Warn(message);
            Send(LogType.Warn, message);
        }

        public static void Error(string msg, int depth = 1)
        {
            string message = CallerName(depth + 1) + msg;
            logger.Error(message);
            Send(LogType.Error, message);
        }

        public static void Fatal(string msg, int depth = 1)
        {
            string message = CallerName(depth + 1) + msg;
            logger.Fatal(message);
            Send(LogType.Fatal, message);
        }

        private static void Send(LogType lt, string message)
        {
            if ((int)lt >= LoggingLevel)
            {
                MessageManager.Send(PDU(lt, message));
            }
        }

        private static LoggingStruct PDU(LogType lt, string message)
        {
            LoggingStruct pdu = new LoggingStruct
            {
                type = lt,
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
