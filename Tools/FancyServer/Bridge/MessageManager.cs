using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using FancyServer.NotifyForm;
using FancyServer.Nursery;
using FancyServer.Messenger;

namespace FancyServer.Bridge
{

    enum MessageType
    {
        action = 1,     // 互相告知工作状态   ↑↓ 
        setting = 2,    // 设置               ↓
        logging = 3,    // 日志、错误消息      ↑
        nursery = 4,    // Nursery           ↑↓      
    }
    
    
    /// <summary>
    /// 负责上层消息的封装与下层消息的解封
    /// </summary>
    partial class MessageManager
    {
        struct MessageStruct
        {
            public MessageType type;    // 消息类型
            public string content;      // 消息内容
        }


        public static void Init()
        {
            PipeMessenger.InitPipeServer();
            NurseryManager.InitProcessInformationSender();
        }

        public static void Receive(string message)
        {
            try
            {
                MessageStruct ms = JsonConvert.DeserializeObject<MessageStruct>(message);
                switch (ms.type)
                {
                    case MessageType.setting:
                        SettingManager.Deal(ms.content);
                        break;
                    case MessageType.action:
                        ActionManager.Deal(ms.content);
                        break;
                    case MessageType.nursery:
                        NurseryManager.Deal(ms.content);
                        break;
                    //case MessageType.log: break;
                    default:
                        LoggingManager.Error("Invalid message type");
                        break;
                }
            }
            catch (JsonReaderException e)
            {
                LoggingManager.Warn(e.Message);
                LoggingManager.Error("Deserialize MessageSettingStruct failed.");
            }
            catch (JsonSerializationException e)
            {
                LoggingManager.Warn(e.Message);
                LoggingManager.Error("Deserialize MessageSettingStruct failed.");
            }
        }

        public static void CloseServer()
        {
            NurseryManager.CloseSender();
            PipeMessenger.ClosePipe();
        }
    }
}
