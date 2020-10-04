using System;
using System.Diagnostics;
using System.Threading;

class Detector
{
    public static void Main()
    {
        foreach (Process process in Process.GetProcesses())
        {
            if (process.ProcessName == "NurseryLauncher") {
                Console.WriteLine("爸爸我找到啦：{0}", process.Id);
            }
        }
        Console.WriteLine("爸爸我……");
        Thread launcher = new Thread(LauncherMonitor)
        {
            Name = "launcher_monitir",
            IsBackground = true
        };
        launcher.Start();
    }

    private static void LauncherMonitor()
    {
        while (true)
        {
            bool isLauncherAlive = false;
            foreach (Process p in Process.GetProcesses())
            {
                if (p.ProcessName == "NurseryLauncher")
                {
                    isLauncherAlive = true;
                }
            }
            if (!isLauncherAlive)
            {
                Process process = new Process();
                process.StartInfo.FileName = @"C:\Users\Administrator\OneDrive\文档\Visual Studio 2019\FancyToys\Assets\Tools\NurseryLauncher.exe";
                process.Start();
                Console.WriteLine(process.Id);
            }
            Thread.Sleep(1000);
        }
    }
}
