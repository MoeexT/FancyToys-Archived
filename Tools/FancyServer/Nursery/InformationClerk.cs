using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

using FancyServer.Log;

namespace FancyServer.Nursery
{
    struct InformationStruct
    {
        public int pid;
        public string processName;
        public float cpu;
        public int memory;
    }
    class InformationClerk
    {
        private static Thread senderThread;
        private static int threadSleepSpan = 1000;
        public static int ThreadSleepSpan { get => threadSleepSpan; set => threadSleepSpan = value; }

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
            bool Clear = true;
            while (true)
            {
                List<Process> plist = ProcessManager.GetProcesses();
                Dictionary<int, InformationStruct> mlist = new Dictionary<int, InformationStruct>();

                foreach (Process ps in plist)
                {
                    try
                    {
                        string pn = ps.ProcessName;
                        PerformanceCounter cpuCounter = new PerformanceCounter("Process", "% Processor Time", pn);
                        PerformanceCounter memCounter = new PerformanceCounter("Process", "Working Set - Private", pn);
                        mlist.Add(ps.Id, new InformationStruct
                        {
                            pid = ps.Id,
                            processName = pn,
                            cpu = cpuCounter.NextValue(),
                            memory = (int)memCounter.NextValue() >> 10
                        });
                        Console.Write($"\r{DateTime.Now:ss:FFF} Process {pn} cpu {cpuCounter.NextValue():F} memory {(int)memCounter.NextValue() >> 10}\t");
                    }
                    catch (InvalidOperationException e)
                    {
                        LogClerk.Warn($"Process exited in unsuitable time, get its information failed: {e.Message}");
                    }
                }
                if (mlist.Count > 0)
                {
                   NurseryManager.Send(mlist);
                    Clear = true;
                }
                else
                {
                    if (Clear)
                    {
                        NurseryManager.Send(mlist);
                        Clear = false;
                    }
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
