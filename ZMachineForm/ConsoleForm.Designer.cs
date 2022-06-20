namespace ZMachineForm
{
    partial class ConsoleForm
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
            this.lowerTextBox = new System.Windows.Forms.TextBox();
            this.consoleMenuStrip = new System.Windows.Forms.MenuStrip();
            this.fileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.recentFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.formatToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fontToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.colorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.upperTextBox = new System.Windows.Forms.TextBox();
            this.consoleMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // lowerTextBox
            // 
            this.lowerTextBox.Enabled = false;
            this.lowerTextBox.Location = new System.Drawing.Point(-9, 223);
            this.lowerTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.lowerTextBox.Multiline = true;
            this.lowerTextBox.Name = "lowerTextBox";
            this.lowerTextBox.ReadOnly = true;
            this.lowerTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.lowerTextBox.Size = new System.Drawing.Size(796, 306);
            this.lowerTextBox.TabIndex = 0;
            this.lowerTextBox.Visible = false;
            this.lowerTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.LowerTextBox_KeyPress);
            // 
            // consoleMenuStrip
            // 
            this.consoleMenuStrip.GripMargin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.consoleMenuStrip.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.consoleMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileMenuItem,
            this.formatToolStripMenuItem});
            this.consoleMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.consoleMenuStrip.Name = "consoleMenuStrip";
            this.consoleMenuStrip.Size = new System.Drawing.Size(800, 40);
            this.consoleMenuStrip.TabIndex = 1;
            this.consoleMenuStrip.Text = "menuStrip1";
            // 
            // fileMenuItem
            // 
            this.fileMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openFileMenuItem,
            this.toolStripSeparator1,
            this.recentFileToolStripMenuItem,
            this.toolStripSeparator2,
            this.exitToolStripMenuItem});
            this.fileMenuItem.Name = "fileMenuItem";
            this.fileMenuItem.Size = new System.Drawing.Size(72, 36);
            this.fileMenuItem.Text = "File";
            // 
            // openFileMenuItem
            // 
            this.openFileMenuItem.Name = "openFileMenuItem";
            this.openFileMenuItem.Size = new System.Drawing.Size(265, 44);
            this.openFileMenuItem.Text = "&Open";
            this.openFileMenuItem.Click += new System.EventHandler(this.FileOpenMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(262, 6);
            // 
            // recentFileToolStripMenuItem
            // 
            this.recentFileToolStripMenuItem.Name = "recentFileToolStripMenuItem";
            this.recentFileToolStripMenuItem.Size = new System.Drawing.Size(265, 44);
            this.recentFileToolStripMenuItem.Text = "Recent File";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(262, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(265, 44);
            this.exitToolStripMenuItem.Text = "&Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.FileExitMenuItem_Click);
            // 
            // formatToolStripMenuItem
            // 
            this.formatToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fontToolStripMenuItem,
            this.colorToolStripMenuItem});
            this.formatToolStripMenuItem.Name = "formatToolStripMenuItem";
            this.formatToolStripMenuItem.Size = new System.Drawing.Size(110, 36);
            this.formatToolStripMenuItem.Text = "Format";
            // 
            // fontToolStripMenuItem
            // 
            this.fontToolStripMenuItem.Name = "fontToolStripMenuItem";
            this.fontToolStripMenuItem.Size = new System.Drawing.Size(206, 44);
            this.fontToolStripMenuItem.Text = "Font";
            this.fontToolStripMenuItem.Click += new System.EventHandler(this.FormatFontMenuItem_Click);
            // 
            // colorToolStripMenuItem
            // 
            this.colorToolStripMenuItem.Name = "colorToolStripMenuItem";
            this.colorToolStripMenuItem.Size = new System.Drawing.Size(206, 44);
            this.colorToolStripMenuItem.Text = "Color";
            this.colorToolStripMenuItem.Click += new System.EventHandler(this.FormatColorMenuItem_Click);
            // 
            // upperTextBox
            // 
            this.upperTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.upperTextBox.Enabled = false;
            this.upperTextBox.Location = new System.Drawing.Point(0, 46);
            this.upperTextBox.Margin = new System.Windows.Forms.Padding(6);
            this.upperTextBox.Multiline = true;
            this.upperTextBox.Name = "upperTextBox";
            this.upperTextBox.ReadOnly = true;
            this.upperTextBox.Size = new System.Drawing.Size(796, 167);
            this.upperTextBox.TabIndex = 2;
            this.upperTextBox.Visible = false;
            // 
            // ConsoleForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 529);
            this.Controls.Add(this.upperTextBox);
            this.Controls.Add(this.lowerTextBox);
            this.Controls.Add(this.consoleMenuStrip);
            this.MainMenuStrip = this.consoleMenuStrip;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "ConsoleForm";
            this.Text = "Z-machine";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ConsoleForm_FormClosing);
            this.Load += new System.EventHandler(this.ConsoleForm_Load);
            this.Resize += new System.EventHandler(this.ConsoleForm_Resize);
            this.consoleMenuStrip.ResumeLayout(false);
            this.consoleMenuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox lowerTextBox;
        private System.Windows.Forms.MenuStrip consoleMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openFileMenuItem;
        private System.Windows.Forms.ToolStripMenuItem formatToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fontToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem colorToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem recentFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.TextBox upperTextBox;
    }
}

