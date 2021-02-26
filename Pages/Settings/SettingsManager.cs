using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FancyToys.Log;
using FancyToys.Messenger;

namespace FancyToys.Pages.Settings
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
    

    class SettingsManager
    {
        
        public static void Deal(string message)
        {
            try
            {
                SettingStruct ss = JsonConvert.DeserializeObject<SettingStruct>(message);
                if (ss.code != SettingCode.OK)
                {
                    LoggingManager.Error($"Apply setting failed: {ss.content}");
                }
            }
            catch (JsonException e)
            {
                LoggingManager.Warn($"Deserialize SettingStruct failed:{e.Message}");
            }
        }

        public static void Send(object sdu)
        {
            SettingStruct? pdu = null;
            switch (sdu)
            {
                case FormSettingStruct fss:
                    pdu = PDU(SettingType.Form, JsonConvert.SerializeObject(fss));
                    break;
                case MessageSettingStruct mss:
                    pdu = PDU(SettingType.Message, JsonConvert.SerializeObject(mss));
                    break;
                case LogSettingStruct lss:
                    pdu = PDU(SettingType.Log, JsonConvert.SerializeObject(lss));
                    break;
                default:
                    LoggingManager.Warn("Invalid setting message type.");
                    break;
            }
            if (pdu != null)
            {
                MessageManager.Send(pdu);
            }
        }

        private static SettingStruct PDU(SettingType st, string message)
        {
            SettingStruct ss = new SettingStruct
            {
                type = st,
                content = message
            };
            return ss;
        }
    }
}
