using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using FancyServer.Bridge;

namespace FancyServer.NotifyForm
{
    
    public partial class NoForm : Form
    {
        delegate void CrossThreadDelegate();  // 跨线程更改NoForm控件的委托
        private static readonly NoForm TheForm = new NoForm();
        static bool once = false;
        private NoForm()
        {
            InitializeComponent();
            Init();
            MessageManager.Init();
        }
        public static NoForm GetTheForm() { return TheForm; }

        private void Init()
        {
            NurserySeparatorItem.Paint += new PaintEventHandler((s, e) =>
            {
                ToolStripSeparator sep = s as ToolStripSeparator;
                e.Graphics.FillRectangle(new SolidBrush(Color.White),
                    0, 0, sep.Width, sep.Height);
                e.Graphics.DrawLine(new Pen(Color.Black),
                    25, sep.Height / 2, sep.Width - 4, sep.Height / 2);
            });
        }

        private void TheNotifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            MouseEventArgs _e = e as MouseEventArgs;
            if (_e.Button == MouseButtons.Left)
            {
                if (!once)
                {
                    once = true;
                    // TODO: delete test code.
                    NoformToNursery.AddProcess(@"C:\puppet.exe");
                    LoggingManager.Trace("trace");
                }

                ActionManager.ReverseShown();
                LoggingManager.Debug("NotifyIcon MouseClick");
            }
        }

        /// <summary>
        /// 退出app
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitMenu_Click(object sender, EventArgs e)
        {
            ActionManager.SendExit();
        }

        public void AddItemToMenu(string pathName)
        {
            /* 
             *  NurseryMenu.DropDownItems ---------> ToolStripItemCollection<ToolStripItem>
             *                                                    ↑
             * ToolStripItem ---> ToolStripSeparator  ------------|
             *          ↓                                         |
             * ToolStripDropDownItem -----------------------------|
             */
            bool hasThisPS = false;
            foreach (ToolStripItem item in NurseryMenu.DropDownItems)
            {
                if (item.Tag != null && item.Tag.Equals(pathName)) { hasThisPS = true; }
            }
            if (!hasThisPS)
            {
                LoggingManager.Info($"Added {pathName} to menu item.");
                var newItem = ActionManager.GetItem(pathName);
                BeginInvoke(new CrossThreadDelegate(() =>
                {
                    NurseryMenu.DropDownItems.Add(newItem);
                }));
                return;
            }
        }

        public void SetNurseryItemCheckState(string pathName, CheckState checkState)
        {
            foreach (ToolStripItem item in NurseryMenu.DropDownItems)
            {
                if (item.Tag!= null && item.Tag.Equals(pathName))
                {
                    BeginInvoke(new CrossThreadDelegate(() =>
                    {
                        (item as ToolStripMenuItem).CheckState = checkState;

                    }));
                    LoggingManager.Info($"Set menu item {item.Text} check state: {checkState}");
                    return;
                }
            }
        }

        public void RemoveNurseryItem(string processName)
        {
            foreach (ToolStripMenuItem item in NurseryMenu.DropDownItems)
            {
                if (item.Text.Equals(processName))
                {
                    BeginInvoke(new CrossThreadDelegate(() =>
                    {
                        NurseryMenu.DropDownItems.Remove(item);
                    }));
                    LoggingManager.Info($"Removed menu item: {processName}");
                    return;
                }
            }
        }

        private void NurseryAddFileItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Coming soon.");
        }
    }   
}
