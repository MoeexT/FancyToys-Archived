using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml;

using FancyToys.Bridge;
using FancyToys.Log;
using FancyToys.Messenger;

namespace FancyToys.Pages
{
    struct ActionStruct
    {
        public bool showWindow;     // 显示主界面
        public bool exitApp;        // 退出程序
    }

    class ActionManager
    {
        public static void Deal(string message)
        {
            try
            {
                ActionStruct acs = JsonConvert.DeserializeObject<ActionStruct>(message);
                if (acs.exitApp)
                {
                    ExitApp();
                }
                if (acs.showWindow)
                {
                    ShowWindow();
                }
                else
                {
                    HideWindow();
                }
            }
            catch (JsonException e)
            {
                LoggingManager.Warn($"Deserialize NurseryActionStruct failed. {e.Message}");
            }
        }

        private static void ShowWindow()
        {
            return;
        }
        private static void HideWindow()
        {
            return;
        }

        private static void ExitApp()
        {
            PipeBridge.Bridge.CLosePipe();
            CoreApplication.Exit();
        }

        public static void TryHideWindow()
        {
            Send(false, false);
        }

        public static void TryExitApp()
        {
            Send(false, true);
            PipeBridge.Bridge.CLosePipe();
            CoreApplication.Exit();
        }

        private static void Send(bool show, bool exit)
        {
            ActionStruct pdu = new ActionStruct
            {
                showWindow = show,
                exitApp = exit,
            };
            MessageManager.Send(pdu);
        }
    }
}
