using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using Windows.Storage;
using Windows.UI.Text;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.ApplicationModel;
using Windows.System;
using System.Net.NetworkInformation;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using System.Net.Sockets;
using Windows.UI.Core;
using Microsoft.Toolkit.Uwp.Helpers;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml.Controls.Primitives;
using FancyToys.Pages.Dialog;

namespace FancyToys.Pages.Nursery
{
    public partial class NurseryPage
    {
        

        private static readonly string MemoryUnit = "KB";
        private static readonly string CPUnit = "%";

        // 进程信息的数据源
        private ObservableCollection<ProcessInformation> InfoList { get; set; }

        // 套接字相关类变量
        private static readonly int port = 626;
        private static byte[] bytes = new byte[1024];
        private static EndPoint point = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
        private static Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        public struct FileProcessStruct
        {
            public string pathname;
            public string args;
            //public Process process;
            public ToggleSwitch tswitch;
            public bool sourcefolder;  // 在源目录执行
            public bool isRunning;
        }

        public struct ClientMessageStruct
        {
            public bool on;
            public string path;
            public string args;
        }
        public struct ServerMessageStruct
        {
            public string process;
            public int pid;
            public float cpu;
            public int memory;
        }

        private async void AddFile(string pathName)
        {
            string fileName = System.IO.Path.GetFileNameWithoutExtension(pathName);
            if (pNersury.ContainsKey(fileName))
            {
                await MessageDialog.Info(fileName + "已存在", "尝试更换文件");
                Debug.WriteLine(fileName + "已存在😄");
                return;
            }

            FileProcessStruct pChild = new FileProcessStruct
            {
                pathname = pathName,
                args = "",
                tswitch = GetSwitch(fileName),
                sourcefolder = true
            };
            
            pNersury.Add(fileName, pChild);
        }

        private ToggleSwitch GetSwitch(string content)
        {
            Debug.WriteLine("创建switch："+content);
            ToggleSwitch twitch = new ToggleSwitch
            {
                IsOn = false,
                OffContent = content + " is Stopped",
                OnContent = content + " is Running",
            };
            twitch.Toggled += Switch_Toggled;
            twitch.RightTapped += Switch_RightTapped;
            ProcessListBox.Items.Add(twitch);
            twitch.ContextFlyout = CommandFlyout;

            return twitch;
        }



        public void SendMessage(bool on, string filename, string args)
        {
            ClientMessageStruct clientMessage = new ClientMessageStruct()
            {
                path = filename,
                args = args,
            };
            try
            {
                Thread receiver = new Thread(ReceiveMessage);
                if (!client.Connected)
                {
                    client.Connect(point);
                }
                if (on)
                {
                    receiver.Start(client);
                    Thread.Sleep(233);
                    clientMessage.on = true;
                    client.Send(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(clientMessage)));
                }
                else
                {
                    clientMessage.on = false;
                    client.Send(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(clientMessage)));
                    Thread.Sleep(233);
                    receiver.Abort();
                }
            }
            catch (Exception e)
            {
                //DialogUtil.Info(e.Message);
                Debug.WriteLine(e.Message);
            }
        }

        public void ReceiveMessage(object skt)
        {
            try
            {
                Socket socket = (Socket)skt;
                int len;
                while (true)
                {
                    len = socket.Receive(bytes);
                    string msg = Encoding.UTF8.GetString(bytes, 0, len);
                    Debug.WriteLine(msg);
                    ServerMessageStruct serverStruct = JsonConvert.DeserializeObject<ServerMessageStruct>(msg);
                    UpdateProcessInformation(serverStruct);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("接收消息时异常:\n"+e.Message + "\n" + e.StackTrace);
                return;
            }
        }

        private void AddProcessInformation(string processName)
        {
            foreach(ProcessInformation pi in InfoList)
            {
                if (pi.Process.Equals(processName))
                {
                    return;
                }
            }
            InfoList.Add(new ProcessInformation()
            {
                Process = processName,
                PID = 0,
                CPU = "0" + CPUnit,
                Memory = "0" + MemoryUnit
            });
            ProcessInformationDataGrid.ItemsSource = InfoList;
        }

        private void UpdateProcessInformation(ServerMessageStruct sms)
        {
            int i;
            int len = InfoList.Count;

            for (i = 0; i < len; i++)
            {
                if (InfoList[i].Process == sms.process)
                {
                    InfoList[i].PID = sms.pid;
                    InfoList[i].CPU = sms.cpu.ToString() + CPUnit;
                    InfoList[i].Memory = sms.memory.ToString() + MemoryUnit;
                    CoreApplication.MainView.Dispatcher.AwaitableRunAsync(() =>
                    {
                        ProcessInformationDataGrid.ItemsSource = InfoList;
                    });
                    break;
                }
            }
        }

        private void RemoveProcessInformation(string fileName)
        {
            for (int i = 0; i < InfoList.Count; i++)
            {
                var item = InfoList[i];
                if (item.Process.Equals(fileName))
                {
                    InfoList.Remove(item);
                    return;
                }
            }
            ProcessInformationDataGrid.ItemsSource = InfoList;
            //ThreadPool.QueueUserWorkItem(delegate
            //{
            //    ///【恢复】SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(
            //    ///【恢复】System.Windows.Application.Current.Dispatcher));
            //    SynchronizationContext.Current.Post(pl =>
            //    {
            //        for (int i = 0; i < InfoList.Count; i++)
            //        {
            //            var item = InfoList[i];
            //            if (item.Process.Equals(fileName))
            //            {
            //                InfoList.Remove(item);
            //                return;
            //            }
            //        }
            //    }, null);
            //});
        }

        /// <summary>
        /// 强制退出所有进程
        /// </summary>
        //[Obsolete]
        //private void StopAllProcesses()
        //{
        //    Dictionary<string, Process> dict = new Dictionary<string, Process>();
        //    foreach (KeyValuePair<string, FileProcessStruct> kv in pNersury)
        //    {
        //        dict.Add(kv.Key, kv.Value.process);
        //    }
        //    foreach (KeyValuePair<string, Process> kv in dict)
        //    {
        //        if (kv.Value != null)
        //        {
        //            kv.Value.Kill();
        //            Debug.WriteLine("killed: " + kv.Key);
        //        }
        //        ModifyNursery(kv.Key, (CheckBox)null);
        //        ModifyNursery(kv.Key, false);
        //    }
        //}


        private void ModifyNursery(string key, string args_)
        {
            FileProcessStruct sct = pNersury[key];
            sct.args = args_;
            pNersury[key] = sct;
        }
        private void ModifyNursery(string key, ToggleSwitch switch_)
        {
            FileProcessStruct sct = pNersury[key];
            sct.tswitch = switch_;
            pNersury[key] = sct;
        }
        private void ModifyNursery(string key, bool isRunning_)
        {
            FileProcessStruct sct = pNersury[key];
            sct.isRunning = isRunning_;
            pNersury[key] = sct;
        }
    }

    public class ProcessInformation
    {
        public string Process { get; set; }
        public int PID { get; set; }
        public string CPU { get; set; }
        public string Memory { get; set; }

        public override string ToString()
        {
            return "{" + Process + ", " + PID.ToString() + ", " +CPU.ToString() + ", " +Memory.ToString() +"}";
        }
    }

    /*public class ProcessInformation : INotifyPropertyChanged
    {
        private string _Process;
        private int _PID;
        private string _CPU;
        private string _Memory;
        public event PropertyChangedEventHandler PropertyChanged;


        public string Process
        {
            get { return _Process; }
            set
            {
                _Process = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Process"));
            }
        }
        public int PID
        {
            get { return _PID; }
            set
            {
                _PID = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PID"));
            }
        }
        // public string Status { get; set; }
        public string CPU
        {
            get { return _CPU; }
            set
            {
                _CPU = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CPU"));
            }
        }
        public string Memory
        {
            get { return _Memory; }
            set
            {
                _Memory = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Memory"));
            }
        }

        public override string ToString()
        {
            return "{" + Process + ", " + PID.ToString() + ", " +CPU.ToString() + ", " +Memory.ToString() +"}";
        }
    }*/
}
