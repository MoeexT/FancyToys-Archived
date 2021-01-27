using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel;
using System.Threading.Tasks;
using System.Security.Principal;
using Windows.UI.Core;
using Windows.Storage;
using Windows.Security.Authentication.Web;
using Windows.ApplicationModel.Core;
using Windows.UI.Core.Preview;
using Windows.UI.Popups;
using System.Diagnostics;
using System.Reflection;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace FancyTest
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private static NamedPipeServerStream server;
        private static NamedPipeClientStream client;
        private static StreamReader reader;
        private static StreamWriter writer;
        private DateTime lastSentMessage;
        private bool waitingForReply;

        private static MainPage mainPage;

        public static MainPage GetMainPage()
        {
            return mainPage;
        }

        public MainPage()
        {
            this.InitializeComponent();
            MethodBase method = new StackTrace().GetFrame(1).GetMethod();
            Debug.WriteLine($"---------------------{method.ReflectedType.Name}:{method.Name}------------------------");
            mainPage = this;
            LaunchServer();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Task.Run((Action)PipeClientThread);
        }

        private async void LaunchServer()
        {
            ApplicationData.Current.LocalSettings.Values["PackageSid"] = WebAuthenticationBroker.GetCurrentApplicationCallbackUri().Host.ToUpper();
            Monitor.Text = WebAuthenticationBroker.GetCurrentApplicationCallbackUri().Host.ToUpper();
            await FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppAsync();
            
        }

        private void PipeServerThread()
        {
            server = new NamedPipeServerStream(
                @"LOCAL\NurseryPipe", 
                PipeDirection.InOut, 1, 
                PipeTransmissionMode.Message, 
                PipeOptions.Asynchronous);

            Log("Waiting for connection");
            server.WaitForConnection();
            Log("Connection established.");

            reader = new StreamReader(server);
            writer = new StreamWriter(server);

            while(true)
            {
                if (!server.IsConnected)
                {
                    /* https://stackoverflow.com/questions/895445/system-io-exception-pipe-is-broken
                     * 不及时断开会报System.IO.Exception: Pipe is brocken.
                     */
                    server.Disconnect();
                    Log("Disconnected from NurseryServer, waiting for another connection.");
                    server.WaitForConnection();
                }
                var responseMessage = reader.ReadLine();
                string appendix;
                if (waitingForReply)
                {
                    waitingForReply = false;
                    TimeSpan duration = DateTime.Now - lastSentMessage;
                    appendix = $"rountrip duration: {duration.TotalMilliseconds}ms";
                } else {
                    appendix = string.Empty;
                }
                Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => Monitor.Text += 
                $"Received '{responseMessage}' from pipe {appendix} {Environment.NewLine}").AsTask().Wait();
            }
        }

        private void PipeClientThread()
        {
            client = new NamedPipeClientStream(".", @"LOCAL\NurseryPipe",
                PipeDirection.InOut, PipeOptions.Asynchronous);
            client.Connect();

            reader = new StreamReader(client);
            writer = new StreamWriter(client);

            while (true)
            {
                if (!client.IsConnected)
                {
                    client.Connect();
                    Log("Conection re-established.");
                }
                string message = reader.ReadLine();
                Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => Monitor.Text +=
                $"Received: {message}{Environment.NewLine}").AsTask().Wait();
            }
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (writer == null)
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    Monitor.Text += "Waiting for connection...\n";
                });
                return;
            }
            lastSentMessage = DateTime.Now;
            waitingForReply = true;
            writer.WriteLine(Message.Text);
            writer.Flush();
        }

        private async void Log(string message)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Monitor.Text += message + "\n";
            });
        }

        private static void ClosePipe()
        {
            if (writer != null) { writer.Close(); }
            if (reader != null) { reader.Close(); }
            if (server != null) { server.Close(); }
        }
    }
}
