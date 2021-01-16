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


namespace FancyServer
{
    public partial class NoForm : Form
    {
        delegate void CrossThreadDelegate();  // 跨线程更改NoForm控件的委托
        private static readonly NoForm TheForm = new NoForm();
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        static bool once = false;
        private NoForm()
        {
            InitializeComponent();
            MessageManager.Init();
        }
        public static NoForm GetTheForm() { return TheForm; }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            AutoScaleMode = AutoScaleMode.Font;
            Visible = false;
        }

        private void TheNotifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            MouseEventArgs _e = e as MouseEventArgs;
            if (_e.Button == MouseButtons.Left)
            {
                if (!once)
                {
                    once = true;
                    MessageManager.NurseryAddProcess(@"C:\puppet.exe");
                }

                if (ActionManager.IsShown)
                {
                    ActionManager.Show();
                }
                else
                {
                    ActionManager.Hide();
                }
                logger.Debug("NotifyIcon_MouseClick");
            }
        }

        /// <summary>
        /// 退出app
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitMenu_Click(object sender, EventArgs e)
        {
            MessageManager.CloseServer();
            logger.Debug("Exit app");
            ActionManager.Exit();
            Application.Exit();
        }


        private void MenuStrip_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            logger.Info(e.CloseReason);
            if (e.CloseReason == ToolStripDropDownCloseReason.ItemClicked)
            {
                e.Cancel = true;
                logger.Debug("item click");
            }
        }

        public void AddItemToMenu(string pathName)
        {
            bool hasThisPS = false;
            foreach (ToolStripMenuItem item in NurseryMenu.DropDownItems)
            {
                if (item.Tag.Equals(pathName)) { hasThisPS = true; }
            }
            if (!hasThisPS)
            {
                logger.Debug("add {0}", pathName);
                var newItem = ActionManager.GetItem(pathName);
                // newItem.CheckState = CheckState.Unchecked;
                BeginInvoke(new CrossThreadDelegate(() =>
                {
                    NurseryMenu.DropDownItems.Add(newItem);
                }));
                return;
            }
        }

        public void SetNurseryItemCheckState(string pathName, CheckState checkState)
        {
            foreach (ToolStripMenuItem item in NurseryMenu.DropDownItems)
            {
                if (item.Tag.Equals(pathName))
                {
                    BeginInvoke(new CrossThreadDelegate(() =>
                    {
                        item.CheckState = checkState;

                    }));
                    logger.Debug("set {0} {1}.", pathName, checkState);
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
                    logger.Debug("set {0} uncheked.", processName);
                    return;
                }
            }
        }
    }
}
