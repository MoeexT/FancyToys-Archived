using Newtonsoft.Json;

using FancyServer.Log;
using FancyServer.Bridge;
using FancyServer.Nursery;
using FancyServer.NotifyForm;
using FancyServer.Utils;

namespace FancyServer.Messenger
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
    internal partial class MessageManager
    {
        struct MessageStruct
        {
            public MessageType type;    // 消息类型
            public string content;      // 消息内容
        }


        public static void Init()
        {
            PipeMessenger.InitPipeServer();
            InformationClerk.InitProcessInformationSender();
        }

        public static void Deal(string message)
        {
            bool success = JsonUtil.ParseStruct<MessageStruct>(message, out MessageStruct ms);
            if (success)
            {
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
                    //case MessageType.log: break;
                    default:
                        LogClerk.Error("Invalid message type");
                        break;
                }
            }
        }

        

        public static void CloseServer()
        {
            InformationClerk.CloseSender();
            PipeMessenger.ClosePipe();
        }
    }
}
