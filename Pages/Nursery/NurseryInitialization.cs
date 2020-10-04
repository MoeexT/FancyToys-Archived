using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.UI.Xaml.Controls;

namespace FancyToys.Pages.Nursery
{
    public partial class NurseryPage
    {
        /// <summary>
        /// 程序初始化
        /// 1. 恢复上次的程序列表
        /// 2. 监控各个线程的详细情况
        /// </summary>
        private async void Initialize()
        {
            // 恢复数据
            pNersury = await FileUtil.FileReader(logger);
            InfoList = new ObservableCollection<ProcessInformation>();
            Dictionary<string, ToggleSwitch> switchDict = new Dictionary<string, ToggleSwitch>();
            foreach (KeyValuePair<string, FileProcessStruct> kv in pNersury)
            {
                ToggleSwitch tswitch = GetSwitch(System.IO.Path.GetFileNameWithoutExtension(kv.Value.pathname));
                switchDict.Add(kv.Key, tswitch);
            }
            foreach (KeyValuePair<string, ToggleSwitch> kv in switchDict)
            {
                ModifyNursery(kv.Key, kv.Value);
            }
        }

        private void InitializeLauncher()
        {
            FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppAsync();
        }
    }
}
