using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

using Newtonsoft.Json;

using FancyServer.Messenger;
using System.Threading;

namespace FancyServer.NotifyForm
{
    struct ActionStruct
    {
        public bool showWindow;     // 显示主界面
        public bool exitApp;        // 退出程序
    }
    class ActionManager
    {

        private static bool isShown = true;
        private static float calmSpan = 1.5f;
        private static DateTime lastReversedShowState = DateTime.Now;

        public static float CalmSpan { get => calmSpan; set => calmSpan = value; }
        public static bool IsShown { get => isShown; set => isShown = value; }
        public static DateTime LastReversedShowState { get => lastReversedShowState; set => lastReversedShowState = value; }

        public static void Deal(string message)
        {
            try
            {
                ActionStruct ac = JsonConvert.DeserializeObject<ActionStruct>(message);
                if (ac.exitApp)
                {
                    ExitApp();
                }
                if (!ac.showWindow)
                {
                    IsShown = false;
                }
            }
            catch(JsonException e)
            {
                LoggingManager.Warn($"Deserialize ActionStruct failed: {e.Message}");
            }

        }

        /// <summary>
        /// Server控制 显示/隐藏前端页面
        /// </summary>
        public static void ShowWindow()
        {
            TimeSpan span = DateTime.Now - LastReversedShowState;
            LoggingManager.Debug($"Reverse shown span: {span.TotalSeconds}");
            LastReversedShowState = DateTime.Now;
            IsShown = true;
            Send(true, false);
        }

        /// <summary>
        /// 退出应用
        /// </summary>
        public static void TryExitApp()
        {
            Send(false, true);
            MessageManager.CloseServer();
            Application.Exit();
        }
        
        private static void ExitApp()
        {
            MessageManager.CloseServer();
            Thread.Sleep(5000);
            Application.Exit();
        }



        /// <summary>
        /// Input   exit    show    eApp    sWin
        ///         true    true    true    fals
        ///   O     true    fals    true    fals
        ///   U     fals    true    fals    true
        ///   T     fals    fals    fals    fals
        ///         --------------------------------------
        ///   P     true    show    true    fals
        ///   U     fals    show    fals    show
        ///         --------------------------------------
        ///   T     exit    show    exit    show&&!exit
        ///      
        /// </summary>
        /// <param name="show">show UWP</param>
        /// <param name="exit">tell UWP to exit application</param>
        /// <returns></returns>
        private static void Send(bool show, bool exit)
        {
            ActionStruct pdu = new ActionStruct
            {
                showWindow = show && !exit,
                exitApp = exit
            };
            MessageManager.Send(pdu);
        }


        public static ToolStripMenuItem GetItem(string pathName, string processName = null)
        {
            if (processName == null || processName.Equals(string.Empty))
            {
                processName = Path.GetFileNameWithoutExtension(pathName);
            }

            ToolStripMenuItem item = new ToolStripMenuItem()
            {
                Text = processName,
                // Tag = pathName,
                AutoToolTip = true,
                ToolTipText = pathName,
                CheckOnClick = false,
                BackColor = Color.White,
                CheckState = CheckState.Unchecked
            };
            item.Click += new EventHandler((s, e) => {
                ToolStripMenuItem i = s as ToolStripMenuItem;
                // 这里有不懂的地方(bug)：`i.CheckState`理应为Unchecked，却总是为Checked 现在没了
                if (i.CheckState == CheckState.Checked)
                {
                    NoformToNursery.StopProcess(i.ToolTipText);
                }
                else
                {
                    NoformToNursery.StartProcess(i.ToolTipText);
                }
            });
            return item;
        }
    }
}
