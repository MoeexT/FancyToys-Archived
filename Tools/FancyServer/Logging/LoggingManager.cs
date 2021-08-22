using Newtonsoft.Json;

using FancyServer.Messenger;


namespace FancyServer.Logging {

    public enum LoggingType {
        Log = 1,
        Std = 2,
        Dialog = 3,
    }

    internal struct LoggingStruct {
        public LoggingType type;
        public string content;
    }

    internal static class LoggingManager {
        public static void Send(object sdu) {
            LoggingStruct? pdu = null;

            switch (sdu) {
                case LogStruct ls:
                    pdu = PDU(LoggingType.Log, JsonConvert.SerializeObject(ls));
                    break;
                case StdStruct ss:
                    pdu = PDU(LoggingType.Std, JsonConvert.SerializeObject(ss));
                    break;
                case DialogStruct ds:
                    pdu = PDU(LoggingType.Dialog, JsonConvert.SerializeObject(ds));
                    break;
            }

            if (pdu != null) {
                MessageManager.Send(pdu);
            }
        }

        private static LoggingStruct PDU(LoggingType lt, string content) {
            LoggingStruct pdu = new LoggingStruct {
                type = lt,
                content = content,
            };
            return pdu;
        }
    }

}