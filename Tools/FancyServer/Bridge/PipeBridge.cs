using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Diagnostics;

using FancyServer.Messenger;
using Windows.Storage;
using System;
using System.Security.Principal;
using System.Security.AccessControl;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel;
using FancyServer.Logging;

namespace FancyServer.Bridge
{
    /// <summary>
    /// 通过管道负责消息的发送与接收
    /// </summary>
    class PipeMessenger
    {
        private static string pipeName = $"Sessions\\{Process.GetCurrentProcess().SessionId}\\AppContainerNamedObjects\\{ApplicationData.Current.LocalSettings.Values["PackageSid"]}\\NurseryPipe";
        //private static readonly string pipeName = @"Sessions\2\AppContainerNamedObjects\S-1-15-2-1581880831-1621145202-3734567036-1267083148-3520691029-3022891357-2621489355\NurseryPipe";
        //private static readonly string pipeName = @"Sessions\2\AppContainerNamedObjects\S-1-15-2-4088229746-2911309162-1607819383-446260963-1424003631-2541624463-62774571\NurseryPipe";
        private static NamedPipeServerStream server;
        private static StreamReader reader;
        private static StreamWriter writer;
        private static Thread serverThread;
        private static bool serverRun = true;
        // S-1-15-2-1581880831-1621145202-3734567036-1267083148-3520691029-3022891357-2621489355
        PipeMessenger() { }

        public static void InitPipeServer()
        {
            serverThread = new Thread(new ThreadStart(PipeServerThread));
            serverThread.Start();
        }

        private static void PipeServerThread()
        {
            server = new NamedPipeServerStream(pipeName,
                PipeDirection.InOut, 1,
                PipeTransmissionMode.Message,
                PipeOptions.Asynchronous, 1024, 1024, null, HandleInheritability.None,
                PipeAccessRights.ChangePermissions);

            PipeSecurity ps = server.GetAccessControl();
            PipeAccessRule clientRule = new PipeAccessRule(
                new SecurityIdentifier("S-1-15-2-1"),
                PipeAccessRights.ReadWrite,
                AccessControlType.Allow);
            PipeAccessRule ownerRule = new PipeAccessRule(
                WindowsIdentity.GetCurrent().Owner,
                PipeAccessRights.FullControl,
                AccessControlType.Allow);
            ps.AddAccessRule(clientRule);
            ps.AddAccessRule(ownerRule);
            server.SetAccessControl(ps);


            LogClerk.Info("Waiting for connection.");
            server.WaitForConnection();
            reader = new StreamReader(server);
            writer = new StreamWriter(server);
            LogClerk.Info($"Connection established: {pipeName}");
            
            while (serverRun)
            {
                //if (!server.IsConnected)
                //{
                //    server.Disconnect();
                //    LogClerk.Info("Disconnected from FancyToys, waiting for its reconnection.");
                //    server.WaitForConnection();
                //    LogClerk.Info("FancyToys reconnected");
                //}
                if (server.IsConnected)
                {
                    string message = reader.ReadLine();
                    if (!string.IsNullOrEmpty(message))
                    {
                        MessageManager.Deal(message);
                    }
                }
                else
                {
                    break;
                }
            }
        }

        private static void PipeConnectionDetector()
        {
            while(true)
            {
                
                if (!server.IsConnected)
                {
                    server.Disconnect();
                    LogClerk.Info("Disconnected from FancyToys, waiting for its reconnection.");
                    server.WaitForConnection();
                    LogClerk.Info("FancyToys reconnected");
                }
            }
        }

        public static bool Post(string message)
        {
            try
            {
                if (!server.IsConnected) { return false; }
                writer.WriteLine(message);
                writer.Flush();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.Message}");
                return false;
            }
        }

        public static void ClosePipe()
        {
            if (server.IsConnected && writer != null) { writer.Close(); }
            if (server.IsConnected && reader != null) { reader.Close(); }
            if (server != null) { server.Close(); }
            if (serverThread != null) {
                serverRun = false;
            }
        }
    }
}
