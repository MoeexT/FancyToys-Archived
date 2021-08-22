using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Diagnostics;
using System.Security.Principal;
using System.Security.AccessControl;

// TODO: 把这个写到文档里，这个很容易忘 C:\Program Files (x86)\Windows Kits\10\UnionMetadata\10.0.19041.0\Windows.winmd
using Windows.Storage;
// using Windows.ApplicationModel;
// using Windows.ApplicationModel.AppService;

using FancyServer.Messenger;


using FancyServer.Logging;


namespace FancyServer.Bridge {

    /// <summary>
    /// 通过管道负责消息的发送与接收
    /// </summary>
    internal static class PipeMessenger {
        private static readonly string PipeName =
            $"Sessions\\{Process.GetCurrentProcess().SessionId}\\AppContainerNamedObjects\\{ApplicationData.Current.LocalSettings.Values["PackageSid"]}\\NurseryPipe";

        //private static readonly string pipeName = @"Sessions\2\AppContainerNamedObjects\S-1-15-2-1581880831-1621145202-3734567036-1267083148-3520691029-3022891357-2621489355\NurseryPipe";
        //private static readonly string pipeName = @"Sessions\2\AppContainerNamedObjects\S-1-15-2-4088229746-2911309162-1607819383-446260963-1424003631-2541624463-62774571\NurseryPipe";
        private static NamedPipeServerStream server;
        private static StreamReader reader;
        private static StreamWriter writer;
        private static Thread serverThread;

        private static bool serverRun = true;

        // S-1-15-2-1581880831-1621145202-3734567036-1267083148-3520691029-3022891357-2621489355

        public static void InitPipeServer() {
            serverThread = new Thread(PipeServerThread);
            serverThread.Start();
        }

        private static void PipeServerThread() {
            server = new NamedPipeServerStream(PipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Message,
                PipeOptions.Asynchronous, 1024, 1024, null, HandleInheritability.None,
                PipeAccessRights.ChangePermissions);

            PipeSecurity ps = server.GetAccessControl();

            PipeAccessRule clientRule = new PipeAccessRule(new SecurityIdentifier("S-1-15-2-1"),
                PipeAccessRights.ReadWrite, AccessControlType.Allow);

            SecurityIdentifier securityIdentifier = WindowsIdentity.GetCurrent().Owner;

            if (!(securityIdentifier is null)) {
                PipeAccessRule ownerRule = new PipeAccessRule(securityIdentifier,
                    PipeAccessRights.FullControl, AccessControlType.Allow);
                ps.AddAccessRule(clientRule);
                ps.AddAccessRule(ownerRule);
            }

            server.SetAccessControl(ps);


            LogClerk.Info("Waiting for connection.");
            server.WaitForConnection();
            reader = new StreamReader(server);
            writer = new StreamWriter(server);
            LogClerk.Info($"Connection established: {PipeName}");

            while (serverRun) {
                //if (!server.IsConnected)
                //{
                //    server.Disconnect();
                //    LogClerk.Info("Disconnected from FancyToys, waiting for its reconnection.");
                //    server.WaitForConnection();
                //    LogClerk.Info("FancyToys reconnected");
                //}
                if (server.IsConnected) {
                    string message = reader.ReadLine();

                    if (!string.IsNullOrEmpty(message)) {
                        MessageManager.Deal(message);
                    }
                } else {
                    break;
                }
            }
        }

        [Obsolete]
        private static void PipeConnectionDetector() {
            while (true) {
                if (!server.IsConnected) {
                    server.Disconnect();
                    LogClerk.Info("Disconnected from FancyToys, waiting for its reconnection.");
                    server.WaitForConnection();
                    LogClerk.Info("FancyToys reconnected");
                }
            }
        }

        public static bool Post(string message) {
            try {
                if (!server.IsConnected) { return false; }
                writer.WriteLine(message);
                writer.Flush();
                return true;
            } catch (Exception e) {
                Console.WriteLine($"{e.Message}");
                return false;
            }
        }

        public static void ClosePipe() {
            if (server.IsConnected && writer != null) { writer.Close(); }
            if (server.IsConnected && reader != null) { reader.Close(); }
            server?.Close();

            if (serverThread != null) {
                serverRun = false;
            }
        }
    }

}