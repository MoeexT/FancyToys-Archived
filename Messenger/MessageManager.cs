using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FancyToys.Pages;
using FancyToys.Pages.Settings;
using FancyToys.Pages.Nursery;
using FancyToys.Bridge;

namespace FancyToys.Pages
{
    enum MessageType
    {
        action = 1,     // 互相告知工作状态   ↑↓ 
        setting = 2,    // 设置               ↓
        logging = 3,    // 日志、错误消息      ↑
        nursery = 4,    // Nursery           ↑↓      
    }
    class MessageManager
    {
        struct MessageStruct
        {
            public MessageType type;    // 消息类型
            public string content;      // 消息内容
        }

        public static void Receive(string message)
        {
            try
            {
                MessageStruct ms = JsonConvert.DeserializeObject<MessageStruct>(message);
                switch (ms.type)
                {
                    case MessageType.setting:
                        SettingsManager.Deal(ms.content);
                        break;
                    case MessageType.action:
                        ActionManager.Deal(ms.content);
                        break;
                    case MessageType.nursery:
                        NurseryManager.Deal(ms.content);
                        break;
                    case MessageType.logging:
                        LoggingManager.Deal(ms.content);
                        break;
                    default:
                        LoggingManager.Warn("Invalid message type");
                        break;
                }
            }
            catch (JsonException e)
            {
                LoggingManager.Warn($"Deserialize MessageSettingStruct failed:{e.Message}");
            }
        }

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
                case Settings.SettingStruct ss:
                    pdu = PDU(MessageType.setting, JsonConvert.SerializeObject(ss));
                    break;
                default:
                    LoggingManager.Warn("Invalid message SDU type");
                    break;
            }
            if (pdu != null)
            {
                PipeBridge.Post(JsonConvert.SerializeObject(pdu));
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
