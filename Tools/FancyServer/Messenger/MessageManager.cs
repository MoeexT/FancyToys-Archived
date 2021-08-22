using FancyServer.Logging;
using FancyServer.Bridge;
using FancyServer.Nursery;
using FancyServer.NotifyForm;
using FancyServer.Settings;

using FancyUtil;


namespace FancyServer.Messenger {

    internal enum MessageType {
        Action = 1, // 互相告知工作状态    ↑↓ 
        Setting = 2, // 设置               ↓
        Logging = 3, // 日志、错误消息      ↑
        Nursery = 4, // Nursery           ↑↓ 
    }


    /// <summary>
    /// 负责上层消息的封装与下层消息的解封
    /// </summary>
    internal static partial class MessageManager {
        private struct MessageStruct {
            public MessageType type; // 消息类型
            public string content; // 消息内容
        }


        public static void Init() {
            PipeMessenger.InitPipeServer();
            InformationClerk.InitProcessInformationSender();
        }

        public static void Deal(string message) {
            bool success = JsonUtil.ParseStruct(message, out MessageStruct ms);

            if (!success) return;

            switch (ms.type) {
                case MessageType.Action:
                    ActionManager.Deal(ms.content);
                    break;
                case MessageType.Setting:
                    SettingsManager.Deal(ms.content);
                    break;
                case MessageType.Logging:
                    LogClerk.Error("Log shouldn't be sent from front-end");
                    break;
                case MessageType.Nursery:
                    NurseryManager.Deal(ms.content);
                    break;
                default:
                    LogClerk.Error("Invalid message type");
                    break;
            }
        }


        public static void CloseServer() {
            InformationClerk.CloseSender();
            PipeMessenger.ClosePipe();
        }
    }

}