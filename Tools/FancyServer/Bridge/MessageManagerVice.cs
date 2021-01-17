using FancyServer.Messenger;
using FancyServer.NotifyForm;
using FancyServer.Nursery;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FancyServer.Bridge
{
    partial class MessageManager
    {

        public static void Send(object sdu)
        {
            MessageStruct? pdu = null;
            switch (sdu) 
            {
                case ActionStruct ass:
                    pdu = PDU(MessageType.action, JsonConvert.SerializeObject(ass));
                    break;
                case LoggingStruct ls:
                    pdu = PDU(MessageType.logging, JsonConvert.SerializeObject(ls));
                    break;
                case NurseryStruct ns:
                    pdu = PDU(MessageType.nursery, JsonConvert.SerializeObject(ns));
                    break;
                case SettingStruct ss:
                    pdu = PDU(MessageType.setting, JsonConvert.SerializeObject(ss));
                    break;
                default:
                    LoggingManager.Error("Invalid message SDU type");
                    break;
            }
            if (pdu != null)
            {
                PipeMessenger.Post(JsonConvert.SerializeObject(pdu));
            }
        }

        private static MessageStruct PDU(MessageType mt, string sdu)
        {
            MessageStruct pdu = new MessageStruct
            {
                type = mt,
                content = sdu
            };
            return pdu;
        } 
    }
}
