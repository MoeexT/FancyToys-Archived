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

using FancyServer.Messenger;
using FancyServer.Nursery;

namespace FancyServer
{

    enum MessageType
    {
        setting = 0,        // 设置               ↓
        operation = 1,      // 互相告知工作状态   ↑↓ 
        nursery = 2,        // Nursery           ↑↓      
        log = 3,            // 日志、错误消息      ↑
    }
    enum MessageCode
    {
        OK,         // 操作成功
        Failed,     // 操作失败
        Unknown,    // 结果未知
    }
    struct MessageStruct
    {
        public MessageType type;    // 消息类型
        public MessageCode code;    // 操作结果
        public string content;      // 消息内容
    }
    /// <summary>
    /// 负责上层消息的封装与下层消息的解封
    /// </summary>
    partial class MessageManager
    {
        struct SettingStruct { }
        struct OperationStruct
        {
            public bool showWindow;     // 显示主界面
            public bool exitApp;        // 退出程序
        }

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();


        public static void Init()
        {
            PipeMessenger.InitPipeServer();
            NurseryManager.InitProcessInformationSender();
        }

        private static void Send(string log)
        {
            PipeMessenger.Post(new MessageStruct
            {
                type = MessageType.log,
                code = MessageCode.Failed,
                content = log
            });
        }

        private static void Send(SettingStruct ss)
        {
            PipeMessenger.Post(new MessageStruct 
            {
                type = MessageType.setting,
                code = MessageCode.OK,
                content = JsonConvert.SerializeObject(ss)
            });
        }

        private static void Send(OperationStruct os)
        {
            PipeMessenger.Post(new MessageStruct 
            {
                type = MessageType.operation,
                code = MessageCode.OK,
                content = JsonConvert.SerializeObject(os)
            });
        }
        /// <summary>
        /// 上层服务调用
        /// </summary>
        /// <param name="mt"></param>
        /// <param name="message"></param>
        public static void Send(MessageType mt, string message)
        {
            PipeMessenger.Post(new MessageStruct 
            { 
                type = mt,
                code = MessageCode.OK,
                content = message
            });
        }

        public static void Receive(MessageStruct ms)
        {
            switch (ms.type)
            {
                case MessageType.setting:
                    DealWIthSetting(ms);
                    break;
                case MessageType.operation:
                    DealWithOperation(ms);
                    break;
                case MessageType.nursery:
                    NurseryManager.Deal(ms.content);
                    break;
                default:
                    DealWithNothing();
                    break;
            }
        }

        private static void DealWIthSetting(MessageStruct ms)
        {
            try
            {
                SettingStruct ss = JsonConvert.DeserializeObject<SettingStruct>(ms.content);
                // TODO: apply settings
                Send(ss);
            }
            catch (JsonReaderException e)
            {
                logger.Warn(e.Message);
                logger.Error("来自前端的无效设置消息");
            }
            catch (JsonSerializationException e)
            {
                logger.Warn(e.Message);
                logger.Error("来自前端的无效设置消息");
            }
        }

        private static void DealWithOperation(MessageStruct ms)
        {
            return;
        }

        private static void DealWithNothing()
        {
            Send("Invalid message type");
        }

        public static void CloseServer()
        {
            NurseryManager.CloseSender();
            PipeMessenger.ClosePipe();
        }
    }
}
