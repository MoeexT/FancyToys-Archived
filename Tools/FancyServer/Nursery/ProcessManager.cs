using NLog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FancyServer.Nursery
{
    /// <summary>
    /// 操作状态码
    /// </summary>
    enum NurseryCode
    {
        OK = 200,               // 成功
        StartOK = 201,          // 已启动
        StopOK = 202,           // 已停止
        StopUncertain = 203,      // 已停止，原因不确定
        DeleteOK = 204,         // 已删除
        AlreadyStopped = 401,   // 该进程已停止
        AlreadyRunning = 402,   // 该进程正在运行
        Forbidden = 403,        // 拒绝
        FileNotExist = 404,     // 该文件不存在
        ProcessNotExist = 406,  // 该进程不存在
        Failed = 500,           // 操作失败
        UnknownError = 501,     // 未知错误
    }

    struct ProcessStruct
    {
        public bool isRunning;
        public Process process;
    }

    /// <summary>
    /// ProcessManager 和 MessageManager 通信用
    /// </summary>
    struct OperateProcessStruct
    {
        public NurseryCode nurseryCode;
        public string processName;
    }

    partial class ProcessManager
    {

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        //private static Dictionary<string, FileStruct> files = new Dictionary<string, FileStruct>();
        private static Dictionary<string, ProcessStruct> processes = new Dictionary<string, ProcessStruct>();
        public static Dictionary<string, string> fpName = new Dictionary<string, string>();

        public ProcessManager() { }


        public static OperateProcessStruct Add(string pathName, string args)
        {
            OperateProcessStruct ops = new OperateProcessStruct { };

            if (!File.Exists(pathName))
            {
                ops.nurseryCode = NurseryCode.FileNotExist;
                logger.Error("File doesn't exist: {0}", pathName);
                return ops;
            }
            if (processes.ContainsKey(pathName) && processes[pathName].isRunning)
            {
                ops.nurseryCode = NurseryCode.AlreadyRunning;
                logger.Warn("Process has been running: {0}[{1}]", pathName, processes[pathName].process.Id);
                return ops;
            }
            GetProcess(pathName, args);
            return ops;
        }

        public static OperateProcessStruct Start(string pathName)
        {
            OperateProcessStruct ops = new OperateProcessStruct
            {
                processName = Path.GetFileNameWithoutExtension(pathName)
            };

            if (!processes.ContainsKey(pathName))
            {
                ops.nurseryCode = NurseryCode.ProcessNotExist;
                logger.Error("Process doesn't not exist: {0}", pathName);
                return ops;
            }
            if (processes.ContainsKey(pathName) && processes[pathName].isRunning)
            {
                ops.nurseryCode = NurseryCode.AlreadyRunning;
                logger.Warn("Process has been running: {0}[{1}]", pathName, processes[pathName].process.Id);
                return ops;
            }

            ProcessStruct ps = processes[pathName];
            Process child = ps.process;
            bool launchOK = child.Start();

            if (!launchOK)
            {
                ops.nurseryCode = NurseryCode.Failed;
                logger.Info("Launch failed: {0}", pathName);
                return ops;
            }
            /* 发现同名进程
            if (!launchStatus && Process.GetProcessesByName(processes[pathName].process.ProcessName).Length > 1)
            {
                Console.WriteLine("重用了");
                // TODO: Change tool-tip-menu-item's state
                return StatusCode.AlreadyRunning;
            }*/
            child.BeginOutputReadLine();
            child.BeginErrorReadLine();

            ps.isRunning = true;
            processes[pathName] = ps;
            fpName[pathName] = child.ProcessName;
            logger.Info("Launched successfully: {0}[{1}]", child.ProcessName, child.Id);
            ops.nurseryCode = NurseryCode.OK;
            ops.processName = processes[pathName].process.ProcessName;
            return ops;
        }

        public static OperateProcessStruct Stop(string pathName)
        {
            OperateProcessStruct ops = new OperateProcessStruct
            {
                nurseryCode = NurseryCode.StopOK
            };
            if (!processes.ContainsKey(pathName))
            {
                ops.nurseryCode = NurseryCode.ProcessNotExist;
                logger.Error("Process doesn't exist: {0}", pathName);
                return ops;
            }
            ProcessStruct ps = processes[pathName];
            ops.nurseryCode = NurseryCode.StopOK;
            ops.processName = ps.process.ProcessName;
            if (ps.process.HasExited)
            {
                ops.nurseryCode = NurseryCode.AlreadyStopped;
                logger.Warn("Process had exited: {0}", pathName);
            }
            ps.process.Kill();
            ps.isRunning = false;
            processes[pathName] = ps;
            return ops;
        }

        public static OperateProcessStruct Remove(string pathName)
        {
            OperateProcessStruct ops = new OperateProcessStruct { };
            if (!processes.ContainsKey(pathName))
            {
                ops.nurseryCode = NurseryCode.ProcessNotExist;
                logger.Error("Process doesn't exist: {0}", pathName);
                return ops;
            }
            Process ps = processes[pathName].process;
            ops.processName = ps.ProcessName;
            ops.nurseryCode = NurseryCode.DeleteOK;

            if (!ps.HasExited) { ps.Kill(); }
            processes.Remove(pathName);
            fpName.Remove(pathName);
            return ops;
        }

        /// <summary>
        /// 打包所有活进程
        /// </summary>
        /// <returns></returns>
        public static List<Process> GetProcesses()
        {
            List<Process> rpl = new List<Process>();
            foreach (ProcessStruct ps in processes.Values)
            {
                if (ps.isRunning)
                {
                    rpl.Add(ps.process);
                }
            }
            return rpl;
        }
    }
}
