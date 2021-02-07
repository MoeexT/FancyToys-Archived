using System;
using System.Drawing;
using System.Windows.Forms;

using FancyServer.Messenger;

namespace FancyServer.NotifyForm
{

    public partial class NoForm : Form
    {
        delegate void CrossThreadDelegate();  // 跨线程更改NoForm控件的委托
        private static NoForm form = new NoForm();

        public static NoForm Form => form;

        private NoForm()
        {
            InitializeComponent();
            Init();
            MessageManager.Init();
        }

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
                ActionManager.ShowWindow();
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
            ActionManager.TryExitApp();
        }

        public bool AddNurseryItem(string pathName)
        {
            /* 
             *  NurseryMenu.DropDownItems ---------> ToolStripItemCollection<ToolStripItem>
             *                                                                     ↑
             * ToolStripItem ---> ToolStripSeparator  -----------------------------|
             *          ↓                                                          |
             * ToolStripDropDownItem ----------------------------------------------|
             */
            bool hasThisPS = false;
            foreach (ToolStripItem item in NurseryMenu.DropDownItems)
            {
                if (item.ToolTipText != null && item.ToolTipText.Equals(pathName)) { hasThisPS = true; }
            }
            if (!hasThisPS)
            {
                var newItem = ActionManager.GetItem(pathName);
                BeginInvoke(new CrossThreadDelegate(() =>
                {
                    NurseryMenu.DropDownItems.Add(newItem);
                }));
                LoggingManager.Info($"Added {pathName}");
            }
            return !hasThisPS;
        }

        public bool SetNurseryItemCheckState(string pathName, CheckState checkState)
        {
            foreach (ToolStripItem item in NurseryMenu.DropDownItems)
            {
                if (item.ToolTipText!= null && item.ToolTipText.Equals(pathName))
                {
                    BeginInvoke(new CrossThreadDelegate(() =>
                    {
                        (item as ToolStripMenuItem).CheckState = checkState;

                    }));
                    LoggingManager.Info($"Set {item.Text} {checkState}");
                    return true;
                }
            }
            return false;
        }

        public bool UpdateNurseryItem(string pathName, string processName)
        {
            foreach (ToolStripItem item in NurseryMenu.DropDownItems)
            {
                if (item.ToolTipText != null && item.ToolTipText.Equals(pathName))
                {
                    BeginInvoke(new CrossThreadDelegate(() =>
                    {
                        item.Text = processName;
                    }));
                    LoggingManager.Info($"Updated {pathName}");
                    return true;
                }
            }
            LoggingManager.Warn($"Menu item not exit while updating it: {pathName}");
            return false;
        }

        public bool RemoveNurseryItem(string pathName)
        {
            foreach (ToolStripItem item in NurseryMenu.DropDownItems)
            {
                if (item.ToolTipText != null && item.ToolTipText.Equals(pathName))
                {
                    BeginInvoke(new CrossThreadDelegate(() =>
                    {
                        NurseryMenu.DropDownItems.Remove(item);
                    }));
                    LoggingManager.Info($"Removed {pathName}");
                    return true;
                }
            }
            LoggingManager.Warn($"Menu item not exit while removing it: {pathName}");
            return false;
        }

        private void NurseryAddFileItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Coming soon.");
        }
    }   
}
