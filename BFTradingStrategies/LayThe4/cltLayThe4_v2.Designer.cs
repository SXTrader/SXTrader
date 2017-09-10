namespace net.sxtrader.bftradingstrategies.LayThe4
{
    partial class cltLayThe4_v2
    {
        /// <summary> 
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.flpCheckboxes = new System.Windows.Forms.FlowLayoutPanel();
            this.cbxActive = new System.Windows.Forms.CheckBox();
            this.cbxAutobetter = new System.Windows.Forms.CheckBox();
            this.dgvLT4 = new System.Windows.Forms.DataGridView();
            this.dgcMatch = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgcPlaytime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgcTotalGoals = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgcWinBack = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgcWinLay = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clhCloseTradeTimer = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clhOpenBetTimer = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clhStopLossTimer = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgcLocalConfig = new System.Windows.Forms.DataGridViewButtonColumn();
            this.clhLivescore1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clhLivescore2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.flpCheckboxes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLT4)).BeginInit();
            this.SuspendLayout();
            // 
            // flpCheckboxes
            // 
            this.flpCheckboxes.Controls.Add(this.cbxActive);
            this.flpCheckboxes.Controls.Add(this.cbxAutobetter);
            this.flpCheckboxes.Dock = System.Windows.Forms.DockStyle.Top;
            this.flpCheckboxes.Location = new System.Drawing.Point(0, 0);
            this.flpCheckboxes.Name = "flpCheckboxes";
            this.flpCheckboxes.Size = new System.Drawing.Size(626, 29);
            this.flpCheckboxes.TabIndex = 0;
            // 
            // cbxActive
            // 
            this.cbxActive.AutoSize = true;
            this.cbxActive.Dock = System.Windows.Forms.DockStyle.Left;
            this.cbxActive.Location = new System.Drawing.Point(3, 3);
            this.cbxActive.Name = "cbxActive";
            this.cbxActive.Size = new System.Drawing.Size(98, 17);
            this.cbxActive.TabIndex = 2;
            this.cbxActive.Text = "Strategy Active";
            this.cbxActive.UseVisualStyleBackColor = true;
            // 
            // cbxAutobetter
            // 
            this.cbxAutobetter.AutoSize = true;
            this.cbxAutobetter.Dock = System.Windows.Forms.DockStyle.Left;
            this.cbxAutobetter.Location = new System.Drawing.Point(107, 3);
            this.cbxAutobetter.Name = "cbxAutobetter";
            this.cbxAutobetter.Size = new System.Drawing.Size(112, 17);
            this.cbxAutobetter.TabIndex = 3;
            this.cbxAutobetter.Text = "Automatic Trading";
            this.cbxAutobetter.UseVisualStyleBackColor = true;
            this.cbxAutobetter.Visible = false;
            // 
            // dgvLT4
            // 
            this.dgvLT4.AllowUserToAddRows = false;
            this.dgvLT4.AllowUserToDeleteRows = false;
            this.dgvLT4.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLT4.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgcMatch,
            this.dgcPlaytime,
            this.dgcTotalGoals,
            this.dgcWinBack,
            this.dgcWinLay,
            this.clhCloseTradeTimer,
            this.clhOpenBetTimer,
            this.clhStopLossTimer,
            this.dgcLocalConfig,
            this.clhLivescore1,
            this.clhLivescore2});
            this.dgvLT4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvLT4.Location = new System.Drawing.Point(0, 29);
            this.dgvLT4.Name = "dgvLT4";
            this.dgvLT4.ReadOnly = true;
            this.dgvLT4.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            this.dgvLT4.ShowEditingIcon = false;
            this.dgvLT4.Size = new System.Drawing.Size(626, 299);
            this.dgvLT4.TabIndex = 1;
            this.dgvLT4.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvLT4_CellValueChanged);
            this.dgvLT4.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dgvLT4_RowsAdded);
            this.dgvLT4.CellMouseEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvLT4_CellMouseEnter);
            this.dgvLT4.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvLT4_CellContentClick);
            // 
            // dgcMatch
            // 
            this.dgcMatch.Frozen = true;
            this.dgcMatch.HeaderText = "Match";
            this.dgcMatch.Name = "dgcMatch";
            this.dgcMatch.ReadOnly = true;
            // 
            // dgcPlaytime
            // 
            this.dgcPlaytime.Frozen = true;
            this.dgcPlaytime.HeaderText = "Playtime";
            this.dgcPlaytime.Name = "dgcPlaytime";
            this.dgcPlaytime.ReadOnly = true;
            // 
            // dgcTotalGoals
            // 
            this.dgcTotalGoals.Frozen = true;
            this.dgcTotalGoals.HeaderText = "TotalGoals";
            this.dgcTotalGoals.Name = "dgcTotalGoals";
            this.dgcTotalGoals.ReadOnly = true;
            // 
            // dgcWinBack
            // 
            this.dgcWinBack.Frozen = true;
            this.dgcWinBack.HeaderText = "Win Back";
            this.dgcWinBack.Name = "dgcWinBack";
            this.dgcWinBack.ReadOnly = true;
            // 
            // dgcWinLay
            // 
            this.dgcWinLay.Frozen = true;
            this.dgcWinLay.HeaderText = "WinLay";
            this.dgcWinLay.Name = "dgcWinLay";
            this.dgcWinLay.ReadOnly = true;
            // 
            // clhCloseTradeTimer
            // 
            this.clhCloseTradeTimer.Frozen = true;
            this.clhCloseTradeTimer.HeaderText = "Close Trade Timer";
            this.clhCloseTradeTimer.Name = "clhCloseTradeTimer";
            this.clhCloseTradeTimer.ReadOnly = true;
            // 
            // clhOpenBetTimer
            // 
            this.clhOpenBetTimer.Frozen = true;
            this.clhOpenBetTimer.HeaderText = "Open Bet Timer";
            this.clhOpenBetTimer.Name = "clhOpenBetTimer";
            this.clhOpenBetTimer.ReadOnly = true;
            // 
            // clhStopLossTimer
            // 
            this.clhStopLossTimer.Frozen = true;
            this.clhStopLossTimer.HeaderText = "Stop/Loss Timer";
            this.clhStopLossTimer.Name = "clhStopLossTimer";
            this.clhStopLossTimer.ReadOnly = true;
            // 
            // dgcLocalConfig
            // 
            this.dgcLocalConfig.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgcLocalConfig.Frozen = true;
            this.dgcLocalConfig.HeaderText = "";
            this.dgcLocalConfig.Name = "dgcLocalConfig";
            this.dgcLocalConfig.ReadOnly = true;
            this.dgcLocalConfig.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dgcLocalConfig.Width = 5;
            // 
            // clhLivescore1
            // 
            this.clhLivescore1.HeaderText = "Livescore 1";
            this.clhLivescore1.Name = "clhLivescore1";
            this.clhLivescore1.ReadOnly = true;
            // 
            // clhLivescore2
            // 
            this.clhLivescore2.HeaderText = "Livescore 2";
            this.clhLivescore2.Name = "clhLivescore2";
            this.clhLivescore2.ReadOnly = true;
            // 
            // cltLayThe4_v2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dgvLT4);
            this.Controls.Add(this.flpCheckboxes);
            this.Name = "cltLayThe4_v2";
            this.Size = new System.Drawing.Size(626, 328);
            this.flpCheckboxes.ResumeLayout(false);
            this.flpCheckboxes.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLT4)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flpCheckboxes;
        private System.Windows.Forms.CheckBox cbxActive;
        private System.Windows.Forms.CheckBox cbxAutobetter;
        private System.Windows.Forms.DataGridView dgvLT4;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcMatch;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcPlaytime;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcTotalGoals;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcWinBack;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcWinLay;
        private System.Windows.Forms.DataGridViewTextBoxColumn clhCloseTradeTimer;
        private System.Windows.Forms.DataGridViewTextBoxColumn clhOpenBetTimer;
        private System.Windows.Forms.DataGridViewTextBoxColumn clhStopLossTimer;
        private System.Windows.Forms.DataGridViewButtonColumn dgcLocalConfig;
        private System.Windows.Forms.DataGridViewTextBoxColumn clhLivescore1;
        private System.Windows.Forms.DataGridViewTextBoxColumn clhLivescore2;
    }
}
