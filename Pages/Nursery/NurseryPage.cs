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
        private static readonly string MemoryUnit = "KB";
        private static readonly string CPUnit = "%";
        // 进程信息数据源
        public ObservableCollection<InformationStruct> InfoList { get; set; }
        private static Dictionary<string, ToggleSwitch> switchCache = new Dictionary<string, ToggleSwitch>();
        private static Dictionary<string, string> fargs = new Dictionary<string, string>();


        private async void TryAddFile(string pathName)
        {
            OperationClerk.AddProcess(pathName);
            if (switchCache.ContainsKey(pathName))
            {
                await MessageDialog.Info("文件已存在", pathName);
                return;
            }
            ToggleSwitch ts = NewSwitch(pathName);
            ProcessListBox.Items.Add(ts);
            switchCache[pathName] = ts;
            fargs[pathName] = "";
        }

        public void AddSwitch(string pathName)
        {
            string processName = Path.GetFileNameWithoutExtension(pathName);
            if (!switchCache.ContainsKey(pathName))
            {
                LoggingManager.Error("The process has started, but its ToggleSwitch can't be found.");
            }
            ToggleSwitch ts = switchCache[pathName];
            switchCache.Remove(pathName);
            fargs.Remove(pathName);
            ts.OnContent = processName + " is Running";
            ts.OffContent = processName + " is Stopped";
            ProcessListBox.Items.Add(ts);
        }

        public void UpdateSwitch(string pathName, string processName)
        {
            foreach(ToggleSwitch ts in ProcessListBox.Items)
            {
                if (ts.Tag.Equals(pathName))
                {
                    // TODO: 不一定能改
                    ts.OnContent = processName + " is Running";
                    ts.OffContent = processName + " is Stopped";
                }
            }
        }

        public void TogglSwitch(string pathName, bool isOn)
        {
            foreach (ToggleSwitch ts in ProcessListBox.Items)
            {
                if (ts.Tag.Equals(pathName))
                {
                    ts.IsOn = isOn;
                }
            }
        }

        public void RemoveSwitch(string pathName)
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
        }

        private ToggleSwitch NewSwitch(string pathName)
        {
            ToggleSwitch twitch = new ToggleSwitch
            {
                IsOn = false,
                Tag = pathName,
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
            InfoList.Clear();
            foreach(InformationStruct ifs in ins)
            {
                InfoList.Add(ifs);
            }
            _ = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
              {
                  ProcessInformationDataGrid.ItemsSource = InfoList;
              });
            //CoreApplication.MainView.Dispatcher.AwaitableRunAsync(() =>
            //{
            //    ProcessInformationDataGrid.ItemsSource = InfoList;
            //});
        }
    }

    //public class ProcessInformation
    //{
    //    public string PID { get; set; }
    //    public string ProcessName { get; set; }
    //    public string CPU { get; set; }
    //    public string Memory { get; set; }

    //    public override string ToString()
    //    {
    //        return $"{{{ProcessName}, {PID}, {CPU}, {Memory}}}";
    //    }
    //}
}
