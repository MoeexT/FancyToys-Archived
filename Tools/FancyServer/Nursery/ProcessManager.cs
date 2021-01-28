using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

using FancyServer.Messenger;

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
        public static Dictionary<string, string> fpName = new Dictionary<string, string>();

        public ProcessManager() { }


        public static bool Add(string pathName)
        {
            if (!File.Exists(pathName))
            {
                LoggingManager.Error($"File doesn't exist: {pathName}");
                return false;
            }
            if (processes.ContainsKey(pathName))
            {
                if (processes[pathName].isRunning)
                {
                    LoggingManager.Warn($"Process has been running: {pathName}[{processes[pathName].process.Id}]");
                }
                else
                {
                    LoggingManager.Warn($"Process already exists, click the switch to run: {pathName}");
                }
                return false;
            }
            AddProcess(pathName);
            return true;
        }

        public static string Start(string pathName, string args)
        {
            if (!processes.ContainsKey(pathName))
            {
                LoggingManager.Error($"Process doesn't not exist: {pathName}");
                return null;
            }
            if (processes.ContainsKey(pathName) && processes[pathName].isRunning)
            {
                LoggingManager.Warn($"Process has been running: {pathName}[{processes[pathName].process.Id}]");
                return processes[pathName].process.ProcessName;
            }
            

            ProcessStruct ps = processes[pathName];
            Process child = ps.process;
            if (!args.Equals(""))
            {
                child.StartInfo.Arguments = args;
            }
            bool launchOK = child.Start();

            if (!launchOK)
            {
                LoggingManager.Error($"Process launch failed: {pathName}");
                return null;
            }
            try
            {
                child.BeginOutputReadLine();
                child.BeginErrorReadLine();
            }
            catch (InvalidOperationException e)
            {
                LoggingManager.Warn($"{e.Message}");
            }

            ps.isRunning = true;
            processes[pathName] = ps;
            fpName[pathName] = child.ProcessName;
            LoggingManager.Info($"Process launched successfully: {child.ProcessName}[{child.Id}]");
            return processes[pathName].process.ProcessName;
        }

        public static bool Stop(string pathName)
        {

            if (!processes.ContainsKey(pathName))
            {
                LoggingManager.Error($"Process doesn't exist: {pathName}");
                return false;
            }
            ProcessStruct ps = processes[pathName];
            if (ps.process.HasExited)
            {
                LoggingManager.Warn($"Process had exited: {pathName}");
            } else
            {
                ps.process.Kill();
            }
            ps.isRunning = false;
            processes[pathName] = ps;
            return true;
        }

        public static void Remove(string pathName)
        {
            if (!processes.ContainsKey(pathName))
            {
                LoggingManager.Error($"Process doesn't exist: {pathName}");
                return;
            }
            Process ps = processes[pathName].process;

            if (!ps.HasExited) { ps.Kill(); }
            processes.Remove(pathName);
            fpName.Remove(pathName);
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
