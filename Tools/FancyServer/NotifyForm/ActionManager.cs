using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

using FancyUtil;

using FancyServer.Messenger;


namespace FancyServer.NotifyForm {

    internal struct ActionStruct {
        public bool showWindow; // 显示主界面
        public bool exitApp; // 退出程序
    }

    internal static class ActionManager {
        private static bool IsShown { get; set; } = true;

        public static DateTime LastReversedShowState { get; set; } = DateTime.Now;

        public static void Deal(string message) {
            bool success = JsonUtil.ParseStruct(message, out ActionStruct ac);

            if (!success) return;
            if (ac.exitApp) ExitApp();
            if (!ac.showWindow) IsShown = false;
        }

        /// <summary>
        /// Server控制 显示/隐藏前端页面
        /// </summary>
        public static void ShowWindow() {
            IsShown = true;
            Send(true, false);
        }

        /// <summary>
        /// 退出应用
        /// </summary>
        public static void TryExitApp() {
            Send(false, true);
            MessageManager.CloseServer();
            Application.Exit();
        }

        private static void ExitApp() {
            MessageManager.CloseServer();
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
        private static void Send(bool show, bool exit) {
            ActionStruct pdu = new ActionStruct {showWindow = show && !exit, exitApp = exit};
            MessageManager.Send(pdu);
        }


        public static ToolStripMenuItem GetItem(string pathName, string processName = null) {
            if (processName == null || processName.Equals(string.Empty)) {
                processName = Path.GetFileNameWithoutExtension(pathName);
            }

            ToolStripMenuItem item = new ToolStripMenuItem() {
                Text = processName,
                // Tag = pathName,
                AutoToolTip = true,
                ToolTipText = pathName,
                CheckOnClick = false,
                BackColor = Color.White,
                CheckState = CheckState.Unchecked
            };

            item.Click += (s, e) => {
                ToolStripMenuItem i = s as ToolStripMenuItem;

                // 这里有不懂的地方(bug)：`i.CheckState`理应为Unchecked，却总是为Checked 现在没了
                if (i?.CheckState == CheckState.Checked) {
                    NoformToOperation.StopProcess(i.ToolTipText);
                } else {
                    NoformToOperation.StartProcess(i?.ToolTipText);
                }
            };
            return item;
        }
    }

}