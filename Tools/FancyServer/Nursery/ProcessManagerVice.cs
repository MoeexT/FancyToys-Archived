using System;
using System.Diagnostics;
using FancyServer.Log;
using FancyServer.Messenger;

namespace FancyServer.Nursery
{
    partial class ProcessManager
    {
        private static Process AddProcess(string pathName)
        {
            Process child = new Process();
            child.StartInfo.RedirectStandardOutput = true;
            child.StartInfo.RedirectStandardError = true;
            child.StartInfo.FileName = pathName;
            child.StartInfo.CreateNoWindow = true;
            child.StartInfo.UseShellExecute = false;
            child.StartInfo.WorkingDirectory = System.IO.Path.GetDirectoryName(pathName);
            child.EnableRaisingEvents = true;  // 这样才会引发 Process.Exited
            child.Exited += OnProcessExit;
            child.OutputDataReceived += (s, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    StdClerk.StdOutput((s as Process).ProcessName, e.Data);
                }
            };
            child.ErrorDataReceived += (s, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    StdClerk.StdError((s as Process).ProcessName, e.Data);
                }
            };

            Processes[pathName] = new ProcessStruct
            {
                process = child,
                isRunning = false
            };
            FPName[pathName] = null;
            LogClerk.Info($"Added {pathName} to local process table.");
            return child;
        }

        private static void OnProcessExit(object sender, EventArgs e)
        {
            Process ps = sender as Process;
            string pathName = ps.StartInfo.FileName;
            ProcessStruct pst = Processes[pathName];
            pst.isRunning = false;
            Processes[pathName] = pst;
            OperationClerk.OnProcessStopped(pathName, FPName[pathName]);
            LogClerk.Info($"Process {FPName[pathName]} exited");
        }
    }
}
