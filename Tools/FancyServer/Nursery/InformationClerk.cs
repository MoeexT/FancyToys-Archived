using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

using FancyServer.Logging;


namespace FancyServer.Nursery {

    internal struct InformationStruct {
        public int pid;
        public string processName;
        public float cpu;
        public int memory;
    }

    internal static class InformationClerk {
        private static Thread senderThread;
        public static int ThreadSleepSpan { get; set; } = 1000;

        public static void InitProcessInformationSender() {
            senderThread = new Thread(new ThreadStart(NurseryInformationThread)) {
                Name = "SenderThread"
            };
            senderThread.Start();
        }

        private static void NurseryInformationThread() {
            bool clear = true;

            while (true) {
                List<Process> plist = ProcessManager.GetProcesses();
                Dictionary<int, InformationStruct> mlist = new Dictionary<int, InformationStruct>();

                foreach (Process ps in plist) {
                    try {
                        string pn = ps.ProcessName;
                        PerformanceCounter cpuCounter = new PerformanceCounter("Process", "% Processor Time", pn);
                        PerformanceCounter memCounter = new PerformanceCounter("Process", "Working Set - Private", pn);

                        mlist.Add(ps.Id, new InformationStruct {
                            pid = ps.Id,
                            processName = pn,
                            cpu = cpuCounter.NextValue(),
                            memory = (int) memCounter.NextValue() >> 10
                        });

                        Console.Write(
                            $"\r{DateTime.Now:ss:FFF} Process {pn} cpu {cpuCounter.NextValue():F} memory {(int) memCounter.NextValue() >> 10}\t");
                    } catch (InvalidOperationException e) {
                        LogClerk.Warn($"Process exited in unsuitable time, get its information failed: {e.Message}");
                    }
                }

                if (mlist.Count > 0) {
                    NurseryManager.Send(mlist);
                    clear = true;
                } else {
                    if (clear) {
                        NurseryManager.Send(mlist);
                        clear = false;
                    }
                }
                Thread.Sleep(ThreadSleepSpan);
            }
        }

        public static void CloseSender() {
            senderThread?.Abort();
        }
    }

}