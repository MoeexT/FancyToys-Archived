using System;

using FancyServer.Logging;
using FancyServer.Messenger;

using FancyUtil;

using Newtonsoft.Json;


namespace FancyServer.Settings {

    internal enum SettingType {
        Form = 1, // ActionManager
        Message = 2, // MessageManager
        Log = 3 // LoggingManager
    }

    internal enum SettingCode {
        OK = 0,
        Failed = 1,
    }

    struct SettingStruct {
        public SettingType type;
        public SettingCode code;
        public string content;
    }

    internal static class SettingsManager {
        private enum FormSettingType { }

        private struct FormSettingStruct {
        }

        private enum MessageSettingType { }

        private struct MessageSettingStruct {
        }

        private enum LogSettingType {
            LogLevel = 1,
            StdLevel = 2,
        }

        private struct LogSettingStruct {
            public LogSettingType type;
            public LogType logLevel;
            public StdType stdLevel;
        }

        public static void Deal(string message) {
            bool success = JsonUtil.ParseStruct(message, out SettingStruct ss);

            if (!success) return;

            switch (ss.type) {
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

        private static void DealWithForm(string sdu) { }

        private static void DealWithMessage(string sdu) { }

        private static void DealWithLog(string sdu) {
            bool success = JsonUtil.ParseStruct(sdu, out LogSettingStruct lss);

            if (!success) return;

            switch (lss.type) {
                case LogSettingType.LogLevel:
                    LogClerk.LogLevel = lss.logLevel;
                    LogClerk.Debug($"Set log level {LogClerk.LogLevel}");
                    break;
                case LogSettingType.StdLevel:
                    StdClerk.StdLevel = lss.stdLevel;
                    LogClerk.Debug($"Set std level {StdClerk.StdLevel}");
                    break;
                default:
                    break;
            }
        }

        [Obsolete]
        private static void Send(object sdu, SettingCode sc) {
            SettingStruct? pdu = null;

            switch (sdu) {
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

            if (pdu != null) {
                MessageManager.Send(pdu);
            }
        }

        private static SettingStruct PDU(SettingType st, SettingCode sc, string sdu) {
            SettingStruct pdu = new SettingStruct {
                type = st,
                code = sc,
                content = sdu
            };
            return pdu;
        }
    }
}
