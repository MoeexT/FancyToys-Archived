
namespace FancyServer.NotifyForm
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
            this.NurseryAddFileItem = new System.Windows.Forms.ToolStripMenuItem();
            this.NurserySeparatorItem = new System.Windows.Forms.ToolStripSeparator();
            this.ExitMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.TheMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // TheNotifyIcon
            // 
            this.TheNotifyIcon.BalloonTipText = "Fancy Toys";
            this.TheNotifyIcon.ContextMenuStrip = this.TheMenu;
            this.TheNotifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("TheNotifyIcon.Icon")));
            this.TheNotifyIcon.Text = "FancyToys";
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
            this.TheMenu.Size = new System.Drawing.Size(136, 52);
            // 
            // NurseryMenu
            // 
            this.NurseryMenu.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.NurseryMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.NurseryAddFileItem,
            this.NurserySeparatorItem});
            this.NurseryMenu.Name = "NurseryMenu";
            this.NurseryMenu.Size = new System.Drawing.Size(135, 24);
            this.NurseryMenu.Text = "Nursery";
            // 
            // NurseryAddFileItem
            // 
            this.NurseryAddFileItem.BackColor = System.Drawing.Color.White;
            this.NurseryAddFileItem.Name = "NurseryAddFileItem";
            this.NurseryAddFileItem.Size = new System.Drawing.Size(132, 26);
            this.NurseryAddFileItem.Text = "Open";
            this.NurseryAddFileItem.Click += new System.EventHandler(this.NurseryAddFileItem_Click);
            // 
            // NurserySeparatorItem
            // 
            this.NurserySeparatorItem.BackColor = System.Drawing.Color.Black;
            this.NurserySeparatorItem.ForeColor = System.Drawing.SystemColors.Control;
            this.NurserySeparatorItem.Name = "NurserySeparatorItem";
            this.NurserySeparatorItem.Size = new System.Drawing.Size(129, 6);
            // 
            // ExitMenu
            // 
            this.ExitMenu.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ExitMenu.Name = "ExitMenu";
            this.ExitMenu.Size = new System.Drawing.Size(135, 24);
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
            this.Text = "FancyServer";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.TheMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NotifyIcon TheNotifyIcon;
        private System.Windows.Forms.ContextMenuStrip TheMenu;
        private System.Windows.Forms.ToolStripMenuItem NurseryMenu;
        private System.Windows.Forms.ToolStripMenuItem ExitMenu;
        private System.Windows.Forms.ToolStripMenuItem NurseryAddFileItem;
        private System.Windows.Forms.ToolStripSeparator NurserySeparatorItem;
    }
}