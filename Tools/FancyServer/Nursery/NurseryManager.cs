using System;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

using Newtonsoft.Json;

using FancyServer.Messenger;

namespace FancyServer.Nursery
{
    enum NurseryType
    {                       //    功能        信息流方向        SDU
        Setting = 1,        // Nursery 设置        ↓      SettingStruct
        Operation = 2,      // 进程开启/关闭       ↓↑      OperationStruct
        Information = 3,    // 进程的具体消息       ↑      InformationStruct
        StandardFile = 4,   // 子进程标准输出流     ↑      StandradFileStruct
    }

    struct NurseryStruct
    {
        public NurseryType type;    // 消息类型
        public string content;      // 消息内容
    }

    /// <summary>
    /// 处理所有由Nursery页面发来的请求
    /// PDU: NurseryStruct
    /// </summary>
    partial class NurseryManager
    {
        private enum SettingCode
        {
            OK = 1,
            Failed = 2,
        }

        private struct SettingStruct
        {
            public SettingCode code;
            public int flushTime;       // 信息刷新时间
        }

        enum OperationCode
        {
            OK = 1,
            Failed = 2,
        }

        enum OperationType
        {
            Add = 1,
            Remove = 2,
            Start = 3,
            Stop = 4,
        }

        private struct OperationStruct
        {
            public OperationType type;
            public OperationCode code;
            public string pathName;     // 要打开的文件
            public string args;         // 参数
            public string processName;  // 打开后的进程名
        }

        private struct InformationStruct
        {
            public int pid;
            public string processName;
            public float cpu;
            public int memory;
        }
        private enum StandardFileType
        {
            StandardIn = 0,
            StandardOutput = 1,
            StandardError = 2,
        }
        private struct StandardFileStruct
        {
            public StandardFileType type;
            public string processName;
            public string content;
        }

        private static Thread senderThread;
        private static int threadSleepSpan = 1000;  // 更新信息时间间隔(ms)

        public static int ThreadSleepSpan { get => threadSleepSpan; set => threadSleepSpan = value; }

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
                        DealWithSetting(ns.content);
                        break;
                    case NurseryType.Operation:
                        DealWithOperation(ns.content);
                        break;
                    //case NurseryType.information:; break;
                    //case NurseryType.StandardOutput:; break;
                    //case NurseryType.StandardOError:; break;
                    default:
                        LoggingManager.Warn($"Invalid nursery type. {content}");
                        break;
                }
            }
            catch (JsonException e)
            {
                LoggingManager.Warn($"Deserialize NurseryStruct failed. {e.Message}");
            }
        }


        private static void DealWithSetting(string content)
        {
            try
            {
                SettingStruct ss = JsonConvert.DeserializeObject<SettingStruct>(content);
                ThreadSleepSpan = ss.flushTime;
                ss.code = SettingCode.OK;
                // Send(ss);
            }
            catch (JsonException e)
            {
                LoggingManager.Warn($"Deserialize SettingStruct failed. {e.Message}");
            }
        }

        private static void DealWithOperation(string content)
        {
            try
            {
                OperationStruct os = JsonConvert.DeserializeObject<OperationStruct>(content);
                switch (os.type)
                {
                    case OperationType.Add:
                        AddProcess(os.pathName);
                        break;
                    case OperationType.Remove:
                        RemoveProcess(os.pathName);
                        break;
                    case OperationType.Start:
                        StartProcess(os.pathName, os.args);
                        break;
                    case OperationType.Stop:
                        StopProcess(os.pathName);
                        break;
                    default:
                        LoggingManager.Warn("Invalid OperationType.");
                        break;
                }
            }
            catch (JsonException e)
            {
                LoggingManager.Warn($"Deserialize OperationStruct failed. {e.Message}");
            }
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

                foreach (Process ps in plist)
                {
                    try
                    {
                        string pn = ps.ProcessName;
                        PerformanceCounter cpuCounter = new PerformanceCounter("Process", "% Processor Time", pn);
                        PerformanceCounter memCounter = new PerformanceCounter("Process", "Working Set - Private", pn);
                        mlist.Add(new InformationStruct
                        {
                            pid = ps.Id,
                            processName = pn,
                            cpu = cpuCounter.NextValue(),
                            memory = (int)memCounter.NextValue() >> 10
                        });
                        LoggingManager.Info($"\r{DateTime.Now:ss:FFF} Process {pn} cpu {cpuCounter.NextValue():F} memory {(int)memCounter.NextValue() >> 10}\t");
                    }
                    catch (InvalidOperationException e)
                    {
                        LoggingManager.Warn($"Process exited in unsuitable time, get its information failed: {e.Message}");
                    }
                }
                if (mlist.Count > 0)
                {
                    Send(mlist);
                }
                Thread.Sleep(ThreadSleepSpan);
            }
        }
        public static void CloseSender()
        {
            if (senderThread != null) { senderThread.Abort(); }
        }
    }
}
