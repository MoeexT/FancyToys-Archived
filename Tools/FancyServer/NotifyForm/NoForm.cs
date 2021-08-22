using System;
using System.Drawing;
using System.Windows.Forms;
using FancyServer.Logging;
using FancyServer.Messenger;

namespace FancyServer.NotifyForm
{

    public partial class NoForm : Form
    {
        private delegate void CrossThreadDelegate();  // 跨线程更改NoForm控件的委托

        public static NoForm Form { get; } = new NoForm();

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
                if (!(s is ToolStripSeparator sep)) return;
                e.Graphics.FillRectangle(new SolidBrush(Color.White), 0, 0, sep.Width, sep.Height);
                e.Graphics.DrawLine(new Pen(Color.Black), 25, sep.Height / 2, sep.Width - 4, sep.Height / 2);
            });
        }

        private void TheNotifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            ActionManager.ShowWindow();
            LogClerk.Debug("NotifyIcon MouseClick");
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
            bool hasThisProcess = false;
            foreach (ToolStripItem item in NurseryMenu.DropDownItems)
            {
                if (item.ToolTipText != null && item.ToolTipText.Equals(pathName)) { hasThisProcess = true; }
            }
            if (!hasThisProcess)
            {
                ToolStripMenuItem newItem = ActionManager.GetItem(pathName);
                BeginInvoke(new CrossThreadDelegate(() =>
                {
                    NurseryMenu.DropDownItems.Add(newItem);
                }));
                LogClerk.Info($"Added {pathName}");
            }
            return !hasThisProcess;
        }

        public bool SetNurseryItemCheckState(string pathName, CheckState checkState)
        {
            foreach (ToolStripItem item in NurseryMenu.DropDownItems)
            {
                if (item.ToolTipText!= null && item.ToolTipText.Equals(pathName))
                {
                    BeginInvoke(new CrossThreadDelegate(() =>
                    {
                        ((ToolStripMenuItem) item).CheckState = checkState;

                    }));
                    LogClerk.Info($"Set {item.Text} {checkState}");
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
                    LogClerk.Info($"Updated {pathName}");
                    return true;
                }
            }
            LogClerk.Warn($"Menu item not exit while updating it: {pathName}");
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
                    LogClerk.Info($"Removed {pathName}");
                    return true;
                }
            }
            LogClerk.Warn($"Menu item not exit while removing it: {pathName}");
            return false;
        }

        private void NurseryAddFileItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Coming soon.");
        }
    }   
}
