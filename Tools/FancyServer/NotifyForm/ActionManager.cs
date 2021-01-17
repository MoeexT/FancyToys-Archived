using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using FancyServer.Bridge;
using System.Reflection;

namespace FancyServer.NotifyForm
{
    struct ActionStruct
    {
        public bool showWindow;     // 显示主界面
        public bool exitApp;        // 退出程序
    }
    class ActionManager
    {
        

        public static bool IsShown = true;
        public static float CalmSpan = 1.5f;
        private static DateTime lastReversedShowState = DateTime.Now;


        public static void Deal(string message)
        {
            try
            {
                ActionStruct ac = JsonConvert.DeserializeObject<ActionStruct>(message);
                if (ac.exitApp)
                {
                    ExitApp();
                }
            }
            catch(JsonReaderException e)
            {
                LoggingManager.Warn(e.Message);
                LoggingManager.Error("Deserialize ActionStruct failed.");
            }
            catch (JsonSerializationException e)
            {
                LoggingManager.Warn(e.Message);
                LoggingManager.Error("Deserialize ActionStruct failed.");
            }

        }

        public static void ExitApp()
        {
            MessageManager.CloseServer();
            Application.Exit();
        }

        /// <summary>
        /// Server控制 显示/隐藏前端页面
        /// </summary>
        public static void ReverseShown()
        {
            TimeSpan span = DateTime.Now - lastReversedShowState;
            lastReversedShowState = DateTime.Now;
            LoggingManager.Debug($"span: {span.TotalSeconds}");
            if (span.TotalSeconds < CalmSpan)
            {
                LoggingManager.Dialog("你点的太快了");
            }
            Send(PDU(!IsShown, false));
        }

        /// <summary>
        /// 退出应用
        /// </summary>
        public static void SendExit()
        {
            Send(PDU(false, true));
            ExitApp();
        }

        private static void Send(ActionStruct pdu)
        {
            MessageManager.Send(pdu);
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
        /// <param name="show"></param>
        /// <param name="exit"></param>
        /// <returns></returns>
        private static ActionStruct PDU(bool show, bool exit)
        {   
            ActionStruct os = new ActionStruct
            {
                showWindow = show && !exit,
                exitApp = exit
            };
            return os;
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
                Tag = pathName,
                CheckOnClick = true,
                BackColor = Color.White,
                CheckState = CheckState.Unchecked
            };
            item.Click += new EventHandler((_s, _e) => {
                ToolStripMenuItem i = (ToolStripMenuItem)_s;
                //Console.WriteLine("{0}, {1}", i.Checked, i.CheckState);
                // 这里有不懂的地方(bug)：`i.CheckState`理应为Unchecked，却总是为Checked
                if (i.CheckState == CheckState.Checked)
                {
                    NoformToNursery.StartProcess((string)i.Tag);
                    //i.CheckState = CheckState.Checked;
                }
                else
                {
                    NoformToNursery.StopProcess((string)i.Tag);
                    //i.CheckState = CheckState.Unchecked;
                }
            });
            return item;
        }
    }
}
