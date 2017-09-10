namespace net.sxtrader.bftradingstrategies.BackThe4
{
    partial class ctlBackThe4_v2
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
            this.dgvBT4 = new System.Windows.Forms.DataGridView();
            this.dgvColumnHead = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cgvColumnPlaytime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgcTotalGoals = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgcWinBack = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgcWinLay = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clhCloseTrade = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clhOpenBetTimer = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clhStopLossTimer = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgcCustomizing = new System.Windows.Forms.DataGridViewButtonColumn();
            this.clhLivescore1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clhLivescore2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.flpCheckboxes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBT4)).BeginInit();
            this.SuspendLayout();
            // 
            // flpCheckboxes
            // 
            this.flpCheckboxes.Controls.Add(this.cbxActive);
            this.flpCheckboxes.Controls.Add(this.cbxAutobetter);
            this.flpCheckboxes.Dock = System.Windows.Forms.DockStyle.Top;
            this.flpCheckboxes.Location = new System.Drawing.Point(0, 0);
            this.flpCheckboxes.Name = "flpCheckboxes";
            this.flpCheckboxes.Size = new System.Drawing.Size(1069, 25);
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
            this.cbxActive.CheckedChanged += new System.EventHandler(this.cbxActive_CheckedChanged);
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
            // dgvBT4
            // 
            this.dgvBT4.AllowUserToAddRows = false;
            this.dgvBT4.AllowUserToDeleteRows = false;
            this.dgvBT4.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvBT4.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgvColumnHead,
            this.cgvColumnPlaytime,
            this.dgcTotalGoals,
            this.dgcWinBack,
            this.dgcWinLay,
            this.clhCloseTrade,
            this.clhOpenBetTimer,
            this.clhStopLossTimer,
            this.dgcCustomizing,
            this.clhLivescore1,
            this.clhLivescore2});
            this.dgvBT4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvBT4.Location = new System.Drawing.Point(0, 25);
            this.dgvBT4.Name = "dgvBT4";
            this.dgvBT4.ReadOnly = true;
            this.dgvBT4.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            this.dgvBT4.ShowEditingIcon = false;
            this.dgvBT4.Size = new System.Drawing.Size(1069, 303);
            this.dgvBT4.TabIndex = 2;
            this.dgvBT4.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvBT4_CellValueChanged);
            this.dgvBT4.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dgvBT4_RowsAdded);
            this.dgvBT4.CellMouseEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvBT4_CellMouseEnter);
            this.dgvBT4.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvBT4_CellContentClick);
            // 
            // dgvColumnHead
            // 
            this.dgvColumnHead.Frozen = true;
            this.dgvColumnHead.HeaderText = "Match";
            this.dgvColumnHead.Name = "dgvColumnHead";
            this.dgvColumnHead.ReadOnly = true;
            // 
            // cgvColumnPlaytime
            // 
            this.cgvColumnPlaytime.Frozen = true;
            this.cgvColumnPlaytime.HeaderText = "Playtime";
            this.cgvColumnPlaytime.Name = "cgvColumnPlaytime";
            this.cgvColumnPlaytime.ReadOnly = true;
            // 
            // dgcTotalGoals
            // 
            this.dgcTotalGoals.Frozen = true;
            this.dgcTotalGoals.HeaderText = "Total Goals";
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
            this.dgcWinLay.HeaderText = "Win Lay";
            this.dgcWinLay.Name = "dgcWinLay";
            this.dgcWinLay.ReadOnly = true;
            // 
            // clhCloseTrade
            // 
            this.clhCloseTrade.Frozen = true;
            this.clhCloseTrade.HeaderText = "Close Trade Timer";
            this.clhCloseTrade.Name = "clhCloseTrade";
            this.clhCloseTrade.ReadOnly = true;
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
            // dgcCustomizing
            // 
            this.dgcCustomizing.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgcCustomizing.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.dgcCustomizing.Frozen = true;
            this.dgcCustomizing.HeaderText = "";
            this.dgcCustomizing.Name = "dgcCustomizing";
            this.dgcCustomizing.ReadOnly = true;
            this.dgcCustomizing.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dgcCustomizing.Text = "Test";
            this.dgcCustomizing.Width = 5;
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
            // ctlBackThe4_v2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.dgvBT4);
            this.Controls.Add(this.flpCheckboxes);
            this.Name = "ctlBackThe4_v2";
            this.Size = new System.Drawing.Size(1069, 328);
            this.flpCheckboxes.ResumeLayout(false);
            this.flpCheckboxes.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBT4)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flpCheckboxes;
        private System.Windows.Forms.DataGridView dgvBT4;
        private System.Windows.Forms.CheckBox cbxActive;
        private System.Windows.Forms.CheckBox cbxAutobetter;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvColumnHead;
        private System.Windows.Forms.DataGridViewTextBoxColumn cgvColumnPlaytime;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcTotalGoals;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcWinBack;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcWinLay;
        private System.Windows.Forms.DataGridViewTextBoxColumn clhCloseTrade;
        private System.Windows.Forms.DataGridViewTextBoxColumn clhOpenBetTimer;
        private System.Windows.Forms.DataGridViewTextBoxColumn clhStopLossTimer;
        private System.Windows.Forms.DataGridViewButtonColumn dgcCustomizing;
        private System.Windows.Forms.DataGridViewTextBoxColumn clhLivescore1;
        private System.Windows.Forms.DataGridViewTextBoxColumn clhLivescore2;

    }
}
