using System;
using System.Diagnostics;

using FancyServer.Messenger;

namespace FancyServer.Nursery
{
    partial class ProcessManager
    {
        private static Process AddProcess(string pathName)
        {
            // if (args == null) { args = string.Empty; }

            Process child = new Process();
            child.StartInfo.RedirectStandardOutput = true;
            child.StartInfo.RedirectStandardError = true;
            child.StartInfo.FileName = pathName;
            child.StartInfo.CreateNoWindow = true;
            child.StartInfo.UseShellExecute = false;
            child.StartInfo.WorkingDirectory = System.IO.Path.GetDirectoryName(pathName);
            child.EnableRaisingEvents = true;  // 这样才会引发 Process.Exited
            child.Exited += OnProcessExit;
            child.OutputDataReceived += new DataReceivedEventHandler(NurseryManager.SendStandardOutput);
            child.ErrorDataReceived += new DataReceivedEventHandler(NurseryManager.SendStandardError);

            processes[pathName] = new ProcessStruct
            {
                process = child,
                isRunning = false
            };
            fpName[pathName] = null;
            LoggingManager.Info($"Added {pathName} to local process table.");
            return child;
        }

        private static void OnProcessExit(object sender, EventArgs e)
        {
            Process ps = sender as Process;
            string pathName = ps.StartInfo.FileName;
            ProcessStruct pst = processes[pathName];
            pst.isRunning = false;
            processes[pathName] = pst;
            NurseryManager.OnProcessStopped(pathName, fpName[pathName]);
            LoggingManager.Info($"Process {fpName[pathName]} exited");
        }
    }
}
