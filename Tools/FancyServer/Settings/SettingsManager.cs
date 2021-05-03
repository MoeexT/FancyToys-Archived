using System;

using Newtonsoft.Json;

using FancyServer.Log;
using FancyUtil;

namespace FancyServer.Messenger
{
    enum SettingType
    {
        Form = 1,       // ActionManager
        Message = 2,    // MessageManager
        Log = 3    // LoggingManager
    }
    enum SettingCode
    {
        OK = 0,
        Failed = 1,
    }
    struct SettingStruct 
    {
        public SettingType type;
        public SettingCode code;
        public string content;
    }
    partial class SettingsManager
    {
        public enum FormSettingType { }
        struct FormSettingStruct { }
        public enum MessageSettingType { }
        struct MessageSettingStruct { }
        public enum LogSettingType
        {
            LogLevel = 1,
            StdLevel = 2,
        }
        public struct LogSettingStruct
        {
            public LogSettingType type;
            public LogType logLevel;
            public StdType stdLevel;
        }

        public static void Deal(string message)
        {
            bool success = JsonUtil.ParseStruct<SettingStruct>(message, out SettingStruct ss);
            if (success)
            {
                switch (ss.type)
                {
                    case SettingType.Form:
                        DealWithForm(ss.content);
                        break;
                    case SettingType.Message:
                        DealWithMessage(ss.content);
                        break;
                    case SettingType.Log:
                        DealWithLog(ss.content);
                        break;
                    default:
                        LogClerk.Error("Invalid setting type");
                        break;
                }
            }
        }

        private static void DealWithForm(string sdu)
        {
            bool success = JsonUtil.ParseStruct<FormSettingStruct>(sdu, out FormSettingStruct  fss);
            if (success)
            {
                return;
            }
        }

        private static void DealWithMessage(string sdu) 
        {
            bool success = JsonUtil.ParseStruct<MessageSettingStruct>(sdu, out MessageSettingStruct mss);
            if (success)
            {
                return;
            }
        }

        private static void DealWithLog(string sdu)
        {
            bool success = JsonUtil.ParseStruct<LogSettingStruct>(sdu, out LogSettingStruct lss);
            if (success)
            {
                switch(lss.type)
                {
                    case LogSettingType.LogLevel:
                        LogClerk.LogLevel = lss.logLevel;
                        LogClerk.Debug($"Set log level {LogClerk.LogLevel}");
                        break;
                    case LogSettingType.StdLevel:
                        StdClerk.StdLevel = lss.stdLevel;
                        LogClerk.Debug($"Set std level {StdClerk.StdLevel}");
                        break;
                }
            }
        }

        [Obsolete]
        private static void Send(object sdu, SettingCode sc)
        {
            SettingStruct? pdu = null;
            switch (sdu)
            {
                case FormSettingStruct fss:
                    pdu = PDU(SettingType.Form, sc, JsonConvert.SerializeObject(fss));
                    break;
                case MessageSettingStruct mss:
                    pdu = PDU(SettingType.Message, sc, JsonConvert.SerializeObject(mss));
                    break;
                case LogSettingStruct lss:
                    pdu = PDU(SettingType.Log, sc, JsonConvert.SerializeObject(lss));
                    break;
                default:
                    LogClerk.Error("Invalid setting message type.", 2);
                    break;
            }
            if (pdu != null)
            {
                MessageManager.Send(pdu);
            }
        }

        private static SettingStruct PDU(SettingType st, SettingCode sc, string sdu)
        {
            SettingStruct pdu = new SettingStruct
            {
                type = st,
                code = sc,
                content = sdu
            };
            return pdu;
        }
    }
}
