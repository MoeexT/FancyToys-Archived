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
using FancyToys.Logging;

namespace FancyToys.Messenger
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
        private static Queue<MessageStruct?> cache = new Queue<MessageStruct?>();

        struct MessageStruct
        {
            public MessageType type;    // 消息类型
            public string content;      // 消息内容
        }

        public static void Initialize()
        {
            PipeBridge.Bridge.PipeOpened += (s, e) =>
            {
                while (cache.Count > 0)
                {
                    PipeBridge.Bridge.Post(JsonConvert.SerializeObject(cache.Dequeue()));
                    LoggingManager.Debug($"dequeue");
                }
            };
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
                case Pages.Settings.SettingStruct ss:
                    pdu = PDU(MessageType.setting, JsonConvert.SerializeObject(ss));
                    break;
                default:
                    LoggingManager.Warn("Invalid message SDU type");
                    break;
            }
            if (pdu != null)
            {
                if (!PipeBridge.Bridge.Post(JsonConvert.SerializeObject(pdu)))
                {
                    cache.Enqueue(pdu);
                    LoggingManager.Debug($"enqueue{pdu?.content}");
                }
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

        public static bool ParseStruct<T>(string content, out T sdu)
        {
            try
            {
                sdu = JsonConvert.DeserializeObject<T>(content);
                return true;
            }
            catch (JsonException e)
            {
                LoggingManager.Warn($"Deserialize object failed: {e.Message}");
                sdu = default;
                return false;
            }
        }
    }
}
