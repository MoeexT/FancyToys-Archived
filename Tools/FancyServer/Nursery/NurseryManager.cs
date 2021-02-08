using Newtonsoft.Json;
using System.Collections.Generic;

using FancyServer.Log;
using FancyServer.Messenger;
using FancyServer.Utils;

namespace FancyServer.Nursery
{
    enum NurseryType
    {                       //    功能        信息流方向        SDU
        Setting = 1,        // Nursery 设置        ↓      SettingStruct
        Operation = 2,      // 进程开启/关闭       ↓↑      OperationStruct
        Information = 3,    // 进程的具体消息       ↑      InformationStruct
    }

    struct NurseryStruct
    {
        //public uint seq;            // 序列号
        public NurseryType type;    // 消息类型
        public string content;      // 消息内容
    }

    partial class NurseryManager
    {

        enum ConfigType
        {
            FlushTime = 1,
        }

        struct ConfigStruct
        {
            public ConfigType type;
            public int flushTime;       // 信息刷新时间
        }

        public static void Deal(string content)
        {
            bool success = JsonUtil.ParseStruct<NurseryStruct>(content, out NurseryStruct ns);
            if (success)
            {
                switch (ns.type)
                {
                    case NurseryType.Setting:
                        DealWithConfig(ns.content);
                        break;
                    case NurseryType.Operation:
                        OperationClerk.Deal(ns.content);
                        break;
                    default:
                        LogClerk.Warn($"Invalid nursery type. {content}");
                        break;
                }
            }
        }

        private static void DealWithConfig(string content)
        {
            bool success = JsonUtil.ParseStruct<ConfigStruct>(content, out ConfigStruct cs);
            if (success)
            {
                switch (cs.type)
                {
                    case ConfigType.FlushTime:
                        InformationClerk.ThreadSleepSpan = cs.flushTime;
                        LogClerk.Info($"Set flush time {InformationClerk.ThreadSleepSpan}");
                        break;
                }
            }
        }

        public static void Send(object sdu)
        {
            NurseryStruct? pdu = null;
            switch (sdu)
            {
                case OperationStruct os:
                    pdu = PDU(NurseryType.Operation, JsonConvert.SerializeObject(os));
                    break;
                case Dictionary<int, InformationStruct> lis:
                    pdu = PDU(NurseryType.Information, JsonConvert.SerializeObject(lis));
                    break;
                default:
                    LogClerk.Warn("Invalid nursery SDU type.", 2);
                    break;
            }
            if (pdu != null)
            {
                MessageManager.Send(pdu);
            }
        }

        private static NurseryStruct PDU(NurseryType nt, string sdu)
        {
            NurseryStruct ns = new NurseryStruct
            {
                type = nt,
                content = sdu
            };
            return ns;
        }
    }
}
