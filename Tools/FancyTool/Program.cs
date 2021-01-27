using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

using System.Windows.Forms;
using System.Threading;

namespace FancyTool
{
    class Program
    {
        static void Main(string[] args)
        {
            string pipeName = $"Sessions\\{Process.GetCurrentProcess().SessionId}\\AppContainerNamedObjects\\{ApplicationData.Current.LocalSettings.Values["PackageSid"]}\\NurseryPipe";
            Console.WriteLine(pipeName);
            Console.ReadKey();
        }

    }
}
