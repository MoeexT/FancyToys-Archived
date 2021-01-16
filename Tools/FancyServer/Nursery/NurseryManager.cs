using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace FancyServer.Nursery
{

    /// <summary>
    /// 处理所有由Nursery页面发来的请求
    /// PDU: NurseryStruct
    /// </summary>
    partial class NurseryManager
    {

        private enum NurseryType
        {                       //    功能        信息流方向        PDU
            Setting = 0,        // Nursery 设置        ↓      SettingStruct
            Operation = 1,      // 进程开启/关闭       ↓↑      OperationStruct
            Information = 2,    // 进程的具体消息       ↑      InformationStruct
            StandardFile = 3,   // 子进程标准输出流     ↑      StandradFileStruct
            Log = 4,            // FancyServer日志     ↑          string
        }

        private struct NurseryStruct
        {
            public NurseryType type;    // 消息类型
            public NurseryCode code;    // 操作结果
            public string content;      // 消息内容
        }

        private struct SettingStruct
        {
            public int flushTime;       // 信息刷新时间
        }
        private struct OperationStruct
        {
            public bool add;            // true: 添加进程；false: do nothing add, remove 不可同时为true
            public bool remove;         // true: 删除进程；false: do nothing 
            public string pathName;     // 要打开的文件
            public string args;         // 参数
            public bool turn;           // true: 打开进程；false: 关闭进程
            public string processName;  // 打开后的进程名
        }

        private struct InformationStruct
        {
            public int pid;
            public string processName;
            public float cpu;
            public int momory;
        }
        private enum StandardFileType
        {
            StandardIn = 0,
            StandardOutput = 1,
            StandardError,
        }
        private struct StandardFileStruct
        {
            public StandardFileType type;
            public string content;
        }

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private static Thread senderThread;
        private static int threadSleepSpan = 1000;  // 更新信息时间间隔(ms)

        /// <summary>
        /// 接受前端消息，只有`Operation`的操作，`Information`操作只能从后台发往前端
        /// 只要`NurseryOperationStruct.delete`置为true，就执行删除操作（此时该进程一定是退出状态）
        /// turn on: 执行并回应
        /// turn off: 只执行不回应；回应操作由进程退出事件触发（进程退出的原因不只有前端的操作）
        /// </summary>
        /// <param name="ms"></param>
        public static void Deal(string content)
        {
            try
            {
                NurseryStruct ns = JsonConvert.DeserializeObject<NurseryStruct>(content);
                switch (ns.type)
                {
                    case NurseryType.Setting:
                        DealWIthSetting(ns.content);
                        break;
                    case NurseryType.Operation:
                        DealWithOperation(ns.content);
                        break;
                    //case NurseryType.information:; break;
                    //case NurseryType.StandardOutput:; break;
                    //case NurseryType.StandardOError:; break;
                    default:
                        DealWithErrorType();
                        break;
                }
            }
            catch (JsonReaderException e)
            {
                logger.Error(e.Message);
                logger.Warn("[Nursery] Invalid message: {0}", content);
            }
            catch (JsonSerializationException e)
            {
                logger.Error(e.Message);
                logger.Warn("[Nursery] Invalid message: {0}", content);
            }
        }


        private static void DealWIthSetting(string content)
        {
            try
            {
                SettingStruct ss = JsonConvert.DeserializeObject<SettingStruct>(content);
                threadSleepSpan = ss.flushTime;
                Send(PDU(ss, NurseryCode.OK));
            }
            catch (JsonReaderException e)
            {
                logger.Error(e.Message);
                logger.Warn("[Nursery.Setting] Invalid message{0}", content);
            }
            catch (JsonSerializationException e)
            {
                logger.Error(e.Message);
                logger.Warn("[Nursery.Setting] Invalid message{0}", content);
            }
        }

        private static void DealWithOperation(string content)
        {
            try
            {
                OperationStruct os = JsonConvert.DeserializeObject<OperationStruct>(content);
                if (os.add && os.remove)
                {
                    Send(PDU("添加与删除不能同时操作", NurseryCode.Failed));
                    return;
                }
                if (os.add)
                {
                    AddProcess(os.pathName, os.args);
                }
                if (os.remove)
                {
                    RemoveProcess(os.pathName);
                    return;
                }
                if (os.turn)
                {
                    StartProcess(os.pathName);
                }
                else
                {
                    StopProcess(os.pathName);
                }
            }
            catch (JsonReaderException e)
            {
                logger.Warn(e.Message);
                logger.Error("[Nursery.Operation] Invalid message{0}", content);
            }
            catch (JsonSerializationException e)
            {
                logger.Warn(e.Message);
                logger.Error("[Nursery.Operation] Invalid message{0}", content);
            }
        }


        private static void DealWithErrorType()
        {
            Send(PDU("Invalid nursery type.", NurseryCode.UnknownError));
        }

        public static void InitProcessInformationSender()
        {
            senderThread = new Thread(new ThreadStart(NurseryInformationThread))
            {
                Name = "SenderThread"
            };
            senderThread.Start();
        }

        private static void NurseryInformationThread()
        {
            while (true)
            {
                List<Process> plist = ProcessManager.GetProcesses();
                List<InformationStruct> mlist = new List<InformationStruct>();
                try
                {
                    foreach (Process ps in plist)
                    {
                        PerformanceCounter cpuCounter = new PerformanceCounter("Process", "% Processor Time", ps.ProcessName);
                        PerformanceCounter memCounter = new PerformanceCounter("Process", "Working Set - Private", ps.ProcessName);
                        mlist.Add(new InformationStruct
                        {
                            pid = ps.Id,
                            processName = ps.ProcessName,
                            cpu = cpuCounter.NextValue(),
                            momory = (int)memCounter.NextValue() >> 10
                        });
                        Console.Write("\rProcess {0} cpu {1:F} memory {2}",ps.ProcessName, cpuCounter.NextValue(), (int)memCounter.NextValue() >> 10);
                    }
                }
                catch (InvalidOperationException e)
                {
                    logger.Error("Get processes' information error: {0}", e.Message);
                }

                if (mlist.Count > 0)
                {
                    Send(PDU(mlist, NurseryCode.OK));
                }
                Thread.Sleep(threadSleepSpan);
            }
        }
        public static void CloseSender()
        {
            if (senderThread != null) { senderThread.Abort(); }
            logger.Info("SenderThread closed");
        }
    }
}
