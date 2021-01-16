
namespace FancyServer
{
    partial class NoForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NoForm));
            this.TheNotifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.TheMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.NurseryMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.ExitMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.TheMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // TheNotifyIcon
            // 
            this.TheNotifyIcon.BalloonTipText = "Fancy Toys";
            this.TheNotifyIcon.ContextMenuStrip = this.TheMenu;
            this.TheNotifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("TheNotifyIcon.Icon")));
            this.TheNotifyIcon.Text = "TheNotifyIcon";
            this.TheNotifyIcon.Visible = true;
            this.TheNotifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TheNotifyIcon_MouseClick);
            // 
            // TheMenu
            // 
            this.TheMenu.BackColor = System.Drawing.Color.White;
            this.TheMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.TheMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.NurseryMenu,
            this.ExitMenu});
            this.TheMenu.Name = "TheMenu";
            this.TheMenu.Size = new System.Drawing.Size(211, 80);
            // 
            // NurseryMenu
            // 
            this.NurseryMenu.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.NurseryMenu.Name = "NurseryMenu";
            this.NurseryMenu.Size = new System.Drawing.Size(210, 24);
            this.NurseryMenu.Text = "Nursery";
            // 
            // ExitMenu
            // 
            this.ExitMenu.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ExitMenu.Name = "ExitMenu";
            this.ExitMenu.Size = new System.Drawing.Size(210, 24);
            this.ExitMenu.Text = "Exit";
            this.ExitMenu.Click += new System.EventHandler(this.ExitMenu_Click);
            // 
            // NoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Name = "NoForm";
            this.ShowInTaskbar = false;
            this.Text = "NoOne";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.TheMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NotifyIcon TheNotifyIcon;
        private System.Windows.Forms.ContextMenuStrip TheMenu;
        private System.Windows.Forms.ToolStripMenuItem NurseryMenu;
        private System.Windows.Forms.ToolStripMenuItem ExitMenu;
    }
}