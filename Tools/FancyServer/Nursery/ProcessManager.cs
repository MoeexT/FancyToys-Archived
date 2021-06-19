using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

using FancyServer.Messenger;
using FancyServer.Logging;

namespace FancyServer.Nursery
{
    /// <summary>
    /// 操作状态码
    /// </summary>
    //enum OperationReason
    //{
    //    AddOK = 200,                // 已添加
    //    StartOK = 201,              // 已启动
    //    StopOK = 202,               // 已停止
    //    StopUncertain = 203,        // 已停止，原因不确定
    //    RemoveOK = 204,             // 已删除
    //    AlreadyStopped = 401,       // 早已停止
    //    AlreadyRunning = 402,       // 该进程正在运行
    //    Forbidden = 403,            // 拒绝
    //    FileNotExist = 404,         // 文件不存在
    //    ProcessNotExist = 406,      // 进程不存在
    //    ProcessAlreadyExists = 407, // 进程已存在
    //    AddFailed = 500,            // 添加失败
    //    StartFailed = 500,          // 启动失败
    //    StopFailed = 500,           // 停止失败
    //    RemoveFailed = 500,         // 删除失败
    //    UnknownError = 501,         // 未知错误
    //}

    struct ProcessStruct
    {
        public bool isRunning;
        public Process process;
    }

    partial class ProcessManager
    {
        //private static Dictionary<string, FileStruct> files = new Dictionary<string, FileStruct>();
        private static Dictionary<string, ProcessStruct> processes = new Dictionary<string, ProcessStruct>();
        private static Dictionary<string, string> fpName = new Dictionary<string, string>();

        public static Dictionary<string, ProcessStruct> Processes { get => processes; set => processes = value; }
        /// <summary>
        /// <pathName, processName>
        /// </summary>
        public static Dictionary<string, string> FPName { get => fpName; set => fpName = value; }

        public ProcessManager() { }


        public static bool Add(string pathName)
        {
            if (!File.Exists(pathName))
            {
                LogClerk.Error($"File doesn't exist: {pathName}");
                return false;
            }
            if (Processes.ContainsKey(pathName))
            {
                if (Processes[pathName].isRunning)
                {
                    LogClerk.Warn($"Process has been running: {pathName}[{Processes[pathName].process.Id}]");
                }
                else
                {
                    LogClerk.Warn($"Process already exists, click the switch to run: {pathName}");
                }
                return false;
            }
            AddProcess(pathName);
            return true;
        }

        public static string Start(string pathName, string args)
        {
            if (!Processes.ContainsKey(pathName))
            {
                LogClerk.Error($"Process doesn't not exist: {pathName}");
                return null;
            }
            if (Processes.ContainsKey(pathName) && Processes[pathName].isRunning)
            {
                LogClerk.Warn($"Process has been running: {pathName}[{Processes[pathName].process.Id}]");
                return Processes[pathName].process.ProcessName;
            }
            

            ProcessStruct ps = Processes[pathName];
            Process child = ps.process;
            if (!args.Equals(""))
            {
                child.StartInfo.Arguments = args;
            }
            bool launchOK = child.Start();

            if (!launchOK)
            {
                LogClerk.Error($"Process launch failed: {pathName}");
                return null;
            }
            try
            {
                child.BeginOutputReadLine();
                child.BeginErrorReadLine();
            }
            catch (InvalidOperationException e)
            {
                LogClerk.Warn($"{e.Message}");
            }

            ps.isRunning = true;
            Processes[pathName] = ps;
            FPName[pathName] = child.ProcessName;
            LogClerk.Info($"Process launched successfully: {child.ProcessName}[{child.Id}]");
            return Processes[pathName].process.ProcessName;
        }

        public static bool Stop(string pathName)
        {

            if (!Processes.ContainsKey(pathName))
            {
                LogClerk.Error($"Process doesn't exist: {pathName}");
                return false;
            }
            ProcessStruct ps = Processes[pathName];
            if (ps.process.HasExited)
            {
                LogClerk.Warn($"Process had exited: {pathName}");
            } else
            {
                ps.process.Kill();
            }
            ps.isRunning = false;
            Processes[pathName] = ps;
            return true;
        }

        public static void Remove(string pathName)
        {
            if (!Processes.ContainsKey(pathName))
            {
                LogClerk.Error($"Process doesn't exist: {pathName}");
                return;
            }
            Process ps = Processes[pathName].process;

            if (!ps.HasExited) { ps.Kill(); }
            Processes.Remove(pathName);
            FPName.Remove(pathName);
        }

        /// <summary>
        /// 打包所有活进程
        /// </summary>
        /// <returns></returns>
        public static List<Process> GetProcesses()
        {
            List<Process> rpl = new List<Process>();
            foreach (ProcessStruct ps in Processes.Values)
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
