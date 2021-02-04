using Newtonsoft.Json;

using FancyServer.NotifyForm;

namespace FancyServer.Messenger
{
    enum SettingType
    {
        Form = 1,       // ActionManager
        Message = 2,    // MessageManager
        Logging    // LoggingManager
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
    partial class SettingManager
    {
        struct FormSettingStruct { }
        struct MessageSettingStruct { }
        struct LoggingSettingStruct
        {
            public LogLevel level;
        }

        public static void Deal(string message)
        {
            try
            {
                SettingStruct ss = JsonConvert.DeserializeObject<SettingStruct>(message);
                switch (ss.type)
                {
                    case SettingType.Form:
                        DealWithForm(ss.content);
                        break;
                    case SettingType.Message:
                        DealWithMessage(ss.content);
                        break;
                    case SettingType.Logging:
                        DealWithLogging(ss.content);
                        break;
                    default:
                        LoggingManager.Error("Invalid setting type");
                        break;
                }
            }
            catch (JsonException e)
            {
                LoggingManager.Warn(e.Message);
                LoggingManager.Error("Deserialize SettingStruct failed.");
            }
        }

        private static void DealWithForm(string sdu)
        {
            try
            {
                FormSettingStruct fss = JsonConvert.DeserializeObject<FormSettingStruct>(sdu);
                // DO nothing
                Send(fss, SettingCode.OK);
            }
            catch (JsonException e)
            {
                LoggingManager.Warn(e.Message);
                LoggingManager.Error("Deserialize FormSettingStruct failed.");
            }
        }

        private static void DealWithMessage(string sdu) 
        {
            try
            {
                MessageSettingStruct mss = JsonConvert.DeserializeObject<MessageSettingStruct>(sdu);
                // DO nothing
                Send(mss, SettingCode.OK);
            }
            catch (JsonException e)
            {
                LoggingManager.Warn(e.Message);
                LoggingManager.Error("Deserialize MessageSettingStruct failed.");
            }
        }

        private static void DealWithLogging(string sdu)
        {
            try
            {
                LoggingSettingStruct lss = JsonConvert.DeserializeObject<LoggingSettingStruct>(sdu);
                LoggingManager.LoggingLevel = lss.level;
                Send(lss, SettingCode.OK);
            }
            catch (JsonException e)
            {
                LoggingManager.Warn(e.Message);
                LoggingManager.Error("Deserialize LoggingSettingStruct failed.");
            }
        }
        
    }
}
