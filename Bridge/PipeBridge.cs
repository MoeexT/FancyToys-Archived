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

using FancyToys.Log;
using System.Diagnostics;

namespace FancyToys.Bridge
{
    class PipeBridge
    {
        private NamedPipeClientStream client;
        private StreamReader reader;
        private StreamWriter writer;

        public delegate void PipeOpenedEventHandler(object sender, PipeOpenedEventArgs e);
        public delegate void PipeClosedEventHandler(object sender, PipeClosedEventArgs e);

        public event PipeOpenedEventHandler PipeOpened;
        public event PipeClosedEventHandler PipeClosed;

        public class PipeOpenedEventArgs: EventArgs
        { }
        public class PipeClosedEventArgs: EventArgs
        { }

        protected virtual void OnPipeOpened(PipeOpenedEventArgs e)
        {
            LoggingManager.Info("FancyServer connected");
            PipeOpened?.Invoke(this, e);
        }
        protected virtual void OnPipeClosed(PipeClosedEventArgs e)
        {
            LoggingManager.Info("FancyServer disconnected");
            PipeClosed?.Invoke(this, e);
        }

        private static PipeBridge bridge = new PipeBridge();

        public static PipeBridge Bridge { get => bridge; }

        private PipeBridge()
        {
        }

        public async void LaunchThenConnectServer()
        {
            ApplicationData.Current.LocalSettings.Values["PackageSid"] = WebAuthenticationBroker.GetCurrentApplicationCallbackUri().Host.ToUpper();
            await FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppAsync();
            await Task.Run((Action)PipeClientThread);
        }

        private void PipeClientThread()
        {
            client = new NamedPipeClientStream(".", @"LOCAL\NurseryPipe",
                PipeDirection.InOut, PipeOptions.Asynchronous);
            LoggingManager.Info("Waiting for FancyServer");
            client.Connect();
            OnPipeOpened(new PipeOpenedEventArgs());

            reader = new StreamReader(client);
            writer = new StreamWriter(client);

            while (true)
            {
                if (!client.IsConnected)
                {
                    OnPipeClosed(new PipeClosedEventArgs());
                    client.Connect();
                    OnPipeOpened(new PipeOpenedEventArgs());
                }
                string message = reader.ReadLine();
                if (!string.IsNullOrEmpty(message))
                {
                    MessageManager.Receive(message);
                }
            }
        }

        public bool Post(string message)
        {
            if (client == null || !client.IsConnected) { return false; }
            writer.WriteLine(message);
            writer.Flush();
            return true;
        }

        public void CLosePipe()
        {
            if (client.IsConnected && writer != null) { writer.Close(); }
            if (client.IsConnected && reader != null) { reader.Close(); }
            if (client != null) { client.Close(); }
        }
    }
}
