using System;
using System.Drawing;
using System.Windows.Forms;

using FancyServer.Messenger;

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
                LoggingManager.Info($"Added {pathName} to menu item.");
                var newItem = ActionManager.GetItem(pathName);
                BeginInvoke(new CrossThreadDelegate(() =>
                {
                    NurseryMenu.DropDownItems.Add(newItem);
                }));
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
                    LoggingManager.Info($"Set menu item {item.Text} check state: {checkState}");
                    return true;
                }
            }
            return false;
        }

        public bool UpdateNurseryItem(string pathName, string processName)
        {
            foreach (ToolStripMenuItem item in NurseryMenu.DropDownItems)
            {
                if (item.ToolTipText.Equals(pathName))
                {
                    BeginInvoke(new CrossThreadDelegate(() =>
                    {
                        item.Text = processName;
                    }));
                    LoggingManager.Info($"Updated menu item: {pathName}");
                    return true;
                }
            }
            LoggingManager.Warn($"Menu item not exit while updating it: {pathName}");
            return false;
        }

        public bool RemoveNurseryItem(string pathName)
        {
            foreach (ToolStripMenuItem item in NurseryMenu.DropDownItems)
            {
                if (item.ToolTipText.Equals(pathName))
                {
                    BeginInvoke(new CrossThreadDelegate(() =>
                    {
                        NurseryMenu.DropDownItems.Remove(item);
                    }));
                    LoggingManager.Info($"Removed menu item: {pathName}");
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
