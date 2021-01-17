using Newtonsoft.Json;

namespace FancyServer.Bridge
{
    partial class SettingManager
    {
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
                case LoggingSettingStruct lss:
                    pdu = PDU(SettingType.Logging, sc, JsonConvert.SerializeObject(lss));
                    break;
                default:
                    LoggingManager.Error("Invalid setting message type.", 2);
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
