// © 2025 Jake John Wenninger. All rights reserved.
// C.A.S.H - Continuous Automated Syncing Helper

namespace C.A.S.H
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lstSources = new System.Windows.Forms.ListBox();
            this.btnAddSource = new System.Windows.Forms.Button();
            this.txtDestination = new System.Windows.Forms.TextBox();
            this.btnBrowseDest = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnToggleAutoSync = new System.Windows.Forms.Button();
            this.btnResetConfig = new System.Windows.Forms.Button();
            this.txtStatus = new System.Windows.Forms.TextBox();
            this.lblUser = new System.Windows.Forms.Label();

            this.SuspendLayout();

            // Dog-themed colors and fonts
            var bgColor = System.Drawing.Color.FromArgb(255, 244, 221); // soft beige
            var accentColor = System.Drawing.Color.FromArgb(153, 102, 51); // warm brown
            var buttonColor = System.Drawing.Color.FromArgb(255, 204, 102); // warm orange
            var fontMain = new System.Drawing.Font("Comic Sans MS", 9.75F, System.Drawing.FontStyle.Bold);
            var fontStatus = new System.Drawing.Font("Segoe UI", 9F);

            // Form1
            this.ClientSize = new System.Drawing.Size(560, 400);
            this.BackColor = bgColor;
            this.Text = "C.A.S.H 🐾 - Continuous Automated Syncing Helper";
            this.Font = fontMain;

            // lblUser
            this.lblUser.AutoSize = true;
            this.lblUser.Location = new System.Drawing.Point(15, 15);
            this.lblUser.Size = new System.Drawing.Size(80, 18);
            this.lblUser.Text = "User: [loading]";
            this.lblUser.ForeColor = accentColor;

            // lblCopyright
            var lblCopyright = new System.Windows.Forms.Label();
            lblCopyright.AutoSize = true;
            lblCopyright.Location = new System.Drawing.Point(370, 15);
            lblCopyright.Text = "© 2025 Jake John Wenninger";
            lblCopyright.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Italic);
            lblCopyright.ForeColor = accentColor;
            this.Controls.Add(lblCopyright);

            // lblVersion
            var lblVersion = new System.Windows.Forms.Label();
            lblVersion.AutoSize = true;
            lblVersion.Location = new System.Drawing.Point(300, 15);
            lblVersion.Text = "v1.2.1"; // 🔁 <-- Change this version string as needed
            lblVersion.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Italic);
            lblVersion.ForeColor = accentColor;
            this.Controls.Add(lblVersion);


            // lstSources
            this.lstSources.FormattingEnabled = true;
            this.lstSources.ItemHeight = 18;
            this.lstSources.Location = new System.Drawing.Point(15, 45);
            this.lstSources.Size = new System.Drawing.Size(420, 110);
            this.lstSources.BackColor = System.Drawing.Color.FromArgb(255, 239, 212);
            this.lstSources.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lstSources.Font = new System.Drawing.Font("Segoe UI", 9F);

            // btnAddSource
            this.btnAddSource.Location = new System.Drawing.Point(445, 45);
            this.btnAddSource.Size = new System.Drawing.Size(90, 30);
            this.btnAddSource.Text = "🐶 Add Source";
            this.btnAddSource.BackColor = buttonColor;
            this.btnAddSource.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddSource.ForeColor = accentColor;
            this.btnAddSource.Font = fontMain;

            // txtDestination
            this.txtDestination.Location = new System.Drawing.Point(15, 165);
            this.txtDestination.Size = new System.Drawing.Size(420, 25);
            this.txtDestination.BackColor = System.Drawing.Color.FromArgb(255, 239, 212);
            this.txtDestination.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDestination.Font = new System.Drawing.Font("Segoe UI", 9F);

            // btnBrowseDest
            this.btnBrowseDest.Location = new System.Drawing.Point(445, 163);
            this.btnBrowseDest.Size = new System.Drawing.Size(90, 30);
            this.btnBrowseDest.Text = "🐾 Browse";
            this.btnBrowseDest.BackColor = buttonColor;
            this.btnBrowseDest.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBrowseDest.ForeColor = accentColor;
            this.btnBrowseDest.Font = fontMain;

            // btnStart
            this.btnStart.Location = new System.Drawing.Point(15, 205);
            this.btnStart.Size = new System.Drawing.Size(110, 35);
            this.btnStart.Text = "Start Copy 🐕‍🦺";
            this.btnStart.BackColor = accentColor;
            this.btnStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStart.ForeColor = System.Drawing.Color.White;
            this.btnStart.Font = fontMain;

            // btnToggleAutoSync
            this.btnToggleAutoSync.Location = new System.Drawing.Point(135, 205);
            this.btnToggleAutoSync.Size = new System.Drawing.Size(140, 35);
            this.btnToggleAutoSync.Text = "Enable Auto-Sync 🦴";
            this.btnToggleAutoSync.BackColor = buttonColor;
            this.btnToggleAutoSync.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnToggleAutoSync.ForeColor = accentColor;
            this.btnToggleAutoSync.Font = fontMain;

            // btnResetConfig
            this.btnResetConfig.Location = new System.Drawing.Point(280, 205);
            this.btnResetConfig.Size = new System.Drawing.Size(120, 35);
            this.btnResetConfig.Text = "Reset Config 🧹";
            this.btnResetConfig.BackColor = buttonColor;
            this.btnResetConfig.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnResetConfig.ForeColor = accentColor;
            this.btnResetConfig.Font = fontMain;

            // txtStatus
            this.txtStatus.Location = new System.Drawing.Point(15, 250);
            this.txtStatus.Multiline = true;
            this.txtStatus.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtStatus.Size = new System.Drawing.Size(520, 130);
            this.txtStatus.BackColor = System.Drawing.Color.FromArgb(255, 239, 212);
            this.txtStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtStatus.Font = fontStatus;
            this.txtStatus.ForeColor = accentColor;
            this.txtStatus.ReadOnly = true;

            // Add controls to form
            this.Controls.Add(this.lstSources);
            this.Controls.Add(this.btnAddSource);
            this.Controls.Add(this.txtDestination);
            this.Controls.Add(this.btnBrowseDest);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.btnToggleAutoSync);
            this.Controls.Add(this.btnResetConfig);
            this.Controls.Add(this.txtStatus);
            this.Controls.Add(this.lblUser);

            this.ResumeLayout(false);
            this.PerformLayout();

            // Wire up events
            this.btnAddSource.Click += new System.EventHandler(this.btnAddSource_Click);
            this.btnBrowseDest.Click += new System.EventHandler(this.btnBrowseDest_Click);
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            this.btnToggleAutoSync.Click += new System.EventHandler(this.btnToggleAutoSync_Click);
            this.btnResetConfig.Click += new System.EventHandler(this.btnResetConfig_Click);
        }

        private System.Windows.Forms.ListBox lstSources;
        private System.Windows.Forms.Button btnAddSource;
        private System.Windows.Forms.TextBox txtDestination;
        private System.Windows.Forms.Button btnBrowseDest;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnToggleAutoSync;
        private System.Windows.Forms.Button btnResetConfig;
        private System.Windows.Forms.TextBox txtStatus;
        private System.Windows.Forms.Label lblUser;
    }
}
