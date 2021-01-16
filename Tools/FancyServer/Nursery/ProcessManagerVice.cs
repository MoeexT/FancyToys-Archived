using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FancyServer.Nursery
{
    partial class ProcessManager
    {
        private static Process GetProcess(string pathName, string args)
        {
            // if (args == null) { args = string.Empty; }

            Process child = new Process();
            child.StartInfo.RedirectStandardOutput = true;
            child.StartInfo.RedirectStandardError = true;
            child.StartInfo.FileName = pathName;
            child.StartInfo.CreateNoWindow = true;
            child.StartInfo.UseShellExecute = false;
            child.StartInfo.Arguments = args;
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
            logger.Info("Added {0} to local process table");
            return child;
        }

        private static void OnProcessExit(object sender, EventArgs e)
        {
            Process ps = sender as Process;
            string pathName = ps.StartInfo.FileName;
            NurseryManager.OnProcessStopped(pathName, fpName[pathName]);
            logger.Info("{0} exited", fpName[pathName]);
        }
    }
}
