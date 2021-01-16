using NLog;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FancyServer
{
    class ActionManager
    {
        public static bool IsShown = true;
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        

        /// <summary>
        /// 显示UWP
        /// </summary>
        public static void Show()
        {
            // TODO 
        }
        /// <summary>
        /// 隐藏UWP
        /// </summary>
        public static void Hide()
        {
            // TODO 
        }
        /// <summary>
        /// 退出应用
        /// </summary>
        public static void Exit()
        {
            // TODO 
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
                    MessageManager.NurseryStartProcess((string)i.Tag);
                    //i.CheckState = CheckState.Checked;
                }
                else
                {
                    MessageManager.NurseryStopProcess((string)i.Tag);
                    //i.CheckState = CheckState.Unchecked;
                }
            });
            return item;
        }
    }
}
