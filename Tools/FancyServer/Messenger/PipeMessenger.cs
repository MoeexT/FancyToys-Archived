using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading;
using Windows.Storage;

namespace FancyServer.Messenger
{
    /// <summary>
    /// 通过管道负责消息的发送与接收
    /// </summary>
    class PipeMessenger
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
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
            logger.Info("Connection established.");

            reader = new StreamReader(client);
            writer = new StreamWriter(client);

            while (true)
            {
                var line = reader.ReadLine();

                //logger.Info(line);
                try
                {
                    MessageStruct ms = JsonConvert.DeserializeObject<MessageStruct>(line);
                    MessageManager.Receive(ms);
                }
                catch (JsonReaderException e)
                {
                    logger.Warn(e.Message);
                    logger.Error("来自前端的无效消息");
                }
                catch (JsonSerializationException e)
                {
                    logger.Warn(e.Message);
                    logger.Error("来自前端的无效消息");
                }
            }
        }

        public static bool Post(MessageStruct ms)
        {
            if (writer == null) { return false; }

            writer.WriteLine(JsonConvert.SerializeObject(ms));
            writer.Flush();
            return true;
        }

        public static void ClosePipe()
        {
            if (writer != null) { writer.Close(); }
            if (reader != null) { reader.Close(); }
            if (client != null) { client.Close(); }
            if (clientThread != null) { clientThread.Abort(); }
            logger.Info("Pipe closed.");
        }
    }


}
