using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Diagnostics;

using FancyServer.Bridge;

namespace FancyServer.Messenger
{
    /// <summary>
    /// 通过管道负责消息的发送与接收
    /// </summary>
    class PipeMessenger
    {
        private static NamedPipeClientStream client;
        private static StreamReader reader;
        private static StreamWriter writer;
        private static Thread clientThread;

        PipeMessenger() { }

        public static void InitPipeServer()
        {
            clientThread = new Thread(new ThreadStart(PipeClientThread));
            clientThread.Name = "ClientPipeThread";
            clientThread.Start();
        }

        private static void PipeClientThread()
        {
            client = new NamedPipeClientStream(".",
                // $"Sessions\\{Process.GetCurrentProcess().SessionId}\\AppContainerNamedObjects\\{ApplicationData.Current.LocalSettings.Values["PackageSid"]}\\NurseryPipe",
                $"Sessions\\{Process.GetCurrentProcess().SessionId}\\AppContainerNamedObjects\\S-1-15-2-2486220046-417657740-3976339917-3362139460-2716223609-868283920-565007427\\NurseryPipe",
                PipeDirection.InOut, PipeOptions.Asynchronous);

            client.Connect();
            LoggingManager.Info("Connection established.");

            reader = new StreamReader(client);
            writer = new StreamWriter(client);

            while (true)
            {
                var line = reader.ReadLine();
                MessageManager.Receive(line);
            }
        }

        public static bool Post(string message)
        {
            if (writer == null) { return false; }

            writer.WriteLine(message);
            writer.Flush();
            return true;
        }

        public static void ClosePipe()
        {
            if (writer != null) { writer.Close(); }
            if (reader != null) { reader.Close(); }
            if (client != null) { client.Close(); }
            if (clientThread != null) { clientThread.Abort(); }
        }
    }
}
