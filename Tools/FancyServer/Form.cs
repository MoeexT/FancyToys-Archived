using System;
using System.Windows.Forms;

namespace NurseryServer
{
    public partial class NoForm : System.Windows.Forms.Form
    {
        public NoForm()
        {
            InitializeComponent();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            this.Visible = false;
        }

        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            ActionManager.Show();
            Console.WriteLine("NotifyIcon_DoubleClick");
        }

        private void NotifyIcon_DoubleClick(object sender, MouseEventArgs e)
        {
            ActionManager.Show();
            Console.WriteLine("NotifyIcon_DoubleClick");
        }

        private void NotifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (ActionManager.IsShown)
            {
                ActionManager.Show();
            }
            else
            {
                ActionManager.Hide();
            }
            Console.WriteLine("NotifyIcon_MouseClick");
        }

        private void ExitMenu_Click(object sender, EventArgs e)
        {
            ActionManager.Exit();
            Application.Exit();
            Console.WriteLine("ExitToolStripMenuItem_Click");
        }

        private void ProcessMenu_Click(object sender, EventArgs e)
        {
            Nursery.DropDownItems.Add(ActionManager.GetItem("puppet"));
        }

        private void MenuStrip_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            Console.WriteLine(e.CloseReason);
            if (e.CloseReason == ToolStripDropDownCloseReason.ItemClicked)
            {
                e.Cancel = true;
                Console.WriteLine("item click");
            }
        }
    }
}
