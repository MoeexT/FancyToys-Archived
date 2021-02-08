using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FancyToys.Log.Nursery
{
    enum NurseryType
    {                       //    功能        信息流方向        SDU
        Setting = 1,        // Nursery 设置        ↓      SettingStruct
        Operation = 2,      // 进程开启/关闭       ↓↑      OperationStruct
        Information = 3,    // 进程的具体消息       ↑      InformationStruct
        //StandardFile = 4,   // 子进程标准输出流     ↑      StandradFileStruct
    }
    
    struct NurseryStruct
    {
        //public uint seq;            // 序列号
        public NurseryType type;    // 消息类型
        public string content;      // 消息内容
    }

    partial class NurseryManager
    {

        public static void Deal(string message)
        {
            bool success = MessageManager.ParseStruct<NurseryStruct>(message, out NurseryStruct ns);
            if (success)
            {
                switch (ns.type)
                {
                    case NurseryType.Setting:
                        ConfigClerk.Deal(ns.content);
                        break;
                    case NurseryType.Operation:
                        OperationClerk.Deal(ns.content);
                        break;
                    case NurseryType.Information:
                        InformationClerk.Deal(ns.content);
                        break;
                    default:
                        LoggingManager.Warn("Invalid NurseryType");
                        break;
                }
            }
        }

        public static void Send(object sdu)
        {
            NurseryStruct? pdu = null;
            switch(sdu)
            {
                case ConfigStruct ss:
                    pdu = PDU(NurseryType.Setting, JsonConvert.SerializeObject(ss));
                    break;
                case OperationStruct os:
                    pdu = PDU(NurseryType.Operation, JsonConvert.SerializeObject(os));
                    break;
                case InformationStruct fs:
                    pdu = PDU(NurseryType.Information, JsonConvert.SerializeObject(fs));
                    break;
                default:
                    LoggingManager.Warn("Invalid NurseryType while sending sdu");
                    break;
            }
            MessageManager.Send(pdu);
        }

        private static NurseryStruct PDU(NurseryType type, string content)
        {
            NurseryStruct ns = new NurseryStruct
            {
                type = type,
                content = content
            };
            return ns;
        }
    }
}
