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
using System.IO;

namespace FancyToys.Pages.Nursery
{
    public partial class NurseryPage
    {
        //private static readonly string MemoryUnit = "KB";
        //private static readonly string CPUnit = "%";
        // 进程信息数据源
        public ObservableCollection<ProcessInformation> InfoList = new ObservableCollection<ProcessInformation>();
        //private static Dictionary<string, ToggleSwitch> switchCache = new Dictionary<string, ToggleSwitch>();
        private static Dictionary<string, string> fargs = new Dictionary<string, string>();


        private async void TryAddFile(string pathName)
        {
            if (fargs.ContainsKey(pathName))
            {
                await MessageDialog.Info("文件已存在", pathName);
                return;
            }
            else
            {
                fargs[pathName] = "";
                OperationClerk.TryAdd(pathName);
            }
            //ToggleSwitch ts = NewSwitch(pathName);
            // ProcessListBox.Items.Add(ts);
            //switchCache[pathName] = ts;
        }

        public void AddSwitch(string pathName)
        {
            string processName = Path.GetFileNameWithoutExtension(pathName);
            _ = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
              {
                  ToggleSwitch ts = NewSwitch(pathName);
                  ts.OnContent = processName + " is Running";
                  ts.OffContent = processName + " is Stopped";
                  ProcessListBox.Items.Add(ts);
              });

        }

        public void UpdateSwitch(string pathName, string processName)
        {
            _ = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                foreach (ToggleSwitch ts in ProcessListBox.Items)
                {
                    if (ts.Tag.Equals(pathName))
                    {
                        ts.OnContent = processName + " is running";
                        ts.OffContent = processName + " stopped";
                    }
                    break;
                }
            });
        }

        public void TogglSwitch(string pathName, bool isOn)
        {
            _ = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                foreach (ToggleSwitch ts in ProcessListBox.Items)
                {
                    if (ts.Tag.Equals(pathName))
                    {
                        ts.IsOn = isOn;
                    }
                }
            });
        }

        public void RemoveSwitch(string pathName)
        {
            fargs.Remove(pathName);
            _ = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                ToggleSwitch ds = null;
                foreach (ToggleSwitch ts in ProcessListBox.Items)
                {
                    if (ts.Tag.Equals(pathName))
                    {
                        ds = ts;
                    }
                }
                if (ds != null)
                {
                    ProcessListBox.Items.Remove(ds);
                }
            });
        }

        private ToggleSwitch NewSwitch(string pathName)
        {
            ToggleSwitch twitch = new ToggleSwitch
            {
                IsOn = false,
                Tag = pathName,
                FontSize = 14,
            };
            twitch.Toggled += Switch_Toggled;
            twitch.ContextFlyout = NewMenu(pathName);

            return twitch;
        }

        private MenuFlyout NewMenu(string pathName)
        {
            MenuFlyout menu = new MenuFlyout();
            MenuFlyoutItem ai = new MenuFlyoutItem
            {
                Icon = new FontIcon { Glyph = "\uE723" },
                Tag = pathName,
                Text = "参数",
            };
            ai.Click += ArgsButton_Click;
            MenuFlyoutItem ri = new MenuFlyoutItem
            {
                Icon = new FontIcon { Glyph = "\uE74D" },
                Tag = pathName,
                Text = "删除",
            };
            ri.Click += DeleteButton_Click;
            menu.Items.Add(ai);
            menu.Items.Add(ri);
            return menu;
        }

        public void UpdateProcessInformation(List<InformationStruct> ins)
        {
            _ = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                InfoList.Clear();
                foreach (InformationStruct ifs in ins)
                {
                    InfoList.Add(new ProcessInformation(ifs));
                }

                // ProcessGrid.ItemsSource = InfoList;
            });
        }

        public class ProcessInformation : INotifyPropertyChanged
        {
            private string process;
            private string pid;
            private string cpu;
            private string memory;
            public string Process
            {
                get => process;
                set
                {
                    process = value;
                    RaisePropertyChanged(nameof(Process));
                }
            }
            public string PID
            {
                get => pid;
                set
                {
                    pid = value;
                    RaisePropertyChanged(nameof(PID));
                }
            }
            public string CPU
            {
                get => cpu;
                set
                {
                    cpu = value;
                    RaisePropertyChanged(nameof(CPU));
                }
            }
            public string Memory
            {
                get => memory;
                set
                {
                    memory = value;
                    RaisePropertyChanged(nameof(Memory));
                }
            }

            private static readonly double GBSize = Math.Pow(2, 30);

            public event PropertyChangedEventHandler PropertyChanged;
            protected void RaisePropertyChanged(string name) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name)); }
            public ProcessInformation() { }
            public ProcessInformation(InformationStruct ifs)
            {
                this.PID = $"{ifs.pid}";
                this.Process = ifs.processName;
                this.CPU = $"{ifs.cpu:F}%";
                this.Memory = ifs.memory < GBSize ? $"{ifs.memory:N0}KB" : $"{ifs.memory >> 10:N0}MB";
            }
            public override string ToString() { return $"{{{Process}, {PID}, {CPU}, {Memory}}}"; }
        }
    }
}
