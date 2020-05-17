namespace ClientLibrary.Utils.Forms
{
    partial class PrimaryClient
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
            this.focusEater = new System.Windows.Forms.TextBox();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.screen = new System.Windows.Forms.PictureBox();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.screenPanel = new System.Windows.Forms.Panel();
            this.fpsLabel = new System.Windows.Forms.Label();
            this.connectionState = new System.Windows.Forms.TextBox();
            this.screenPanelParent = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.screen)).BeginInit();
            this.screenPanel.SuspendLayout();
            this.screenPanelParent.SuspendLayout();
            this.SuspendLayout();
            // 
            // focusEater
            // 
            this.focusEater.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.focusEater.Location = new System.Drawing.Point(0, 480);
            this.focusEater.Name = "focusEater";
            this.focusEater.ReadOnly = true;
            this.focusEater.Size = new System.Drawing.Size(944, 21);
            this.focusEater.TabIndex = 6;
            this.focusEater.Text = "hi";
            // 
            // notifyIcon
            // 
            this.notifyIcon.Text = "Streaming Client";
            this.notifyIcon.Visible = true;
            this.notifyIcon.DoubleClick += new System.EventHandler(this.notifyIcon_DoubleClick);
            // 
            // screen
            // 
            this.screen.Dock = System.Windows.Forms.DockStyle.Fill;
            this.screen.Image = global::ClientLibrary.Properties.Resources.image;
            this.screen.Location = new System.Drawing.Point(0, 0);
            this.screen.Name = "screen";
            this.screen.Size = new System.Drawing.Size(944, 480);
            this.screen.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.screen.TabIndex = 7;
            this.screen.TabStop = false;
            this.screen.DoubleClick += new System.EventHandler(this.screen_DoubleClick);
            this.screen.MouseMove += new System.Windows.Forms.MouseEventHandler(this.screen_MouseMove);
            this.screen.MouseUp += new System.Windows.Forms.MouseEventHandler(this.screen_MouseUp);
            // 
            // screenPanel
            // 
            this.screenPanel.AutoScroll = true;
            this.screenPanel.Controls.Add(this.screen);
            this.screenPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.screenPanel.Location = new System.Drawing.Point(0, 0);
            this.screenPanel.Name = "screenPanel";
            this.screenPanel.Size = new System.Drawing.Size(944, 480);
            this.screenPanel.TabIndex = 9;
            // 
            // fpsLabel
            // 
            this.fpsLabel.AutoSize = true;
            this.fpsLabel.BackColor = System.Drawing.Color.Black;
            this.fpsLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.fpsLabel.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.fpsLabel.ForeColor = System.Drawing.Color.White;
            this.fpsLabel.Location = new System.Drawing.Point(0, 0);
            this.fpsLabel.Name = "fpsLabel";
            this.fpsLabel.Size = new System.Drawing.Size(53, 12);
            this.fpsLabel.TabIndex = 10;
            this.fpsLabel.Text = "FPS : 0";
            // 
            // connectionState
            // 
            this.connectionState.Dock = System.Windows.Forms.DockStyle.Top;
            this.connectionState.Location = new System.Drawing.Point(0, 0);
            this.connectionState.Name = "connectionState";
            this.connectionState.ReadOnly = true;
            this.connectionState.Size = new System.Drawing.Size(944, 21);
            this.connectionState.TabIndex = 10;
            // 
            // screenPanelParent
            // 
            this.screenPanelParent.Controls.Add(this.fpsLabel);
            this.screenPanelParent.Controls.Add(this.screenPanel);
            this.screenPanelParent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.screenPanelParent.Location = new System.Drawing.Point(0, 21);
            this.screenPanelParent.Name = "screenPanelParent";
            this.screenPanelParent.Size = new System.Drawing.Size(944, 480);
            this.screenPanelParent.TabIndex = 11;
            // 
            // PrimaryClient
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(944, 501);
            this.Controls.Add(this.focusEater);
            this.Controls.Add(this.screenPanelParent);
            this.Controls.Add(this.connectionState);
            this.KeyPreview = true;
            this.Name = "PrimaryClient";
            this.Text = "PrimaryClient";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PrimaryClient_FormClosing);
            this.Load += new System.EventHandler(this.PrimaryClient_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.PrimaryClient_KeyDown);
            this.Resize += new System.EventHandler(this.PrimaryClient_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.screen)).EndInit();
            this.screenPanel.ResumeLayout(false);
            this.screenPanelParent.ResumeLayout(false);
            this.screenPanelParent.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox focusEater;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.PictureBox screen;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Panel screenPanel;
        private System.Windows.Forms.Label fpsLabel;
        private System.Windows.Forms.TextBox connectionState;
        private System.Windows.Forms.Panel screenPanelParent;
    }
}