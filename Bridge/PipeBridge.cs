using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Security.Authentication.Web;
using Windows.Storage;
using Windows.UI.Core;

using FancyToys.Pages;
using System.Diagnostics;

namespace FancyToys.Bridge
{
    class PipeBridge
    {
        private static NamedPipeClientStream client;
        private static StreamReader reader;
        private static StreamWriter writer;

        public static async void LaunchThenConnectServer()
        {
            ApplicationData.Current.LocalSettings.Values["PackageSid"] = WebAuthenticationBroker.GetCurrentApplicationCallbackUri().Host.ToUpper();
            await FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppAsync();
            await Task.Run((Action)PipeClientThread);
        }

        private static void PipeClientThread()
        {
            client = new NamedPipeClientStream(".", @"LOCAL\NurseryPipe",
                PipeDirection.InOut, PipeOptions.Asynchronous);
            LoggingManager.Info("等待FancyServer");
            client.Connect();
            LoggingManager.Info("FancyServer已连接");

            reader = new StreamReader(client);
            writer = new StreamWriter(client);

            while (true)
            {
                if (client.IsConnected)
                {
                    string message = reader.ReadLine();
                    if (!string.IsNullOrEmpty(message))
                    {
                        MessageManager.Receive(message);
                    }
                }
            }
        }

        public static bool Post(string message)
        {
            if (writer == null) { return false; }
            writer.WriteLine(message);
            writer.Flush();
            return true;
        }

        public static void CLosePipe()
        {
            if (client.IsConnected && writer != null) { writer.Close(); }
            if (client.IsConnected && reader != null) { reader.Close(); }
            if (client != null) { client.Close(); }
        }
    }
}
