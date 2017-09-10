namespace net.sxtrader.bftradingstrategies.bfuestrategy
{
    partial class ctlLayDraw_v2
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ctlLayDraw_v2));
            this.flpCheckboxes = new System.Windows.Forms.FlowLayoutPanel();
            this.cbxActive = new System.Windows.Forms.CheckBox();
            this.btnCheckTrades = new System.Windows.Forms.Button();
            this.dgvLTD = new System.Windows.Forms.DataGridView();
            this.clhMatch = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clhPlaytime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clhScore = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clhWinBack = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clhWinLay = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clhCloseTradeTimer = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clhOpenBetTimer = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clhStopLossTimer = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clhLocalConfig = new System.Windows.Forms.DataGridViewButtonColumn();
            this.clhLivescore1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clhLivescore2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.flpCheckboxes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLTD)).BeginInit();
            this.SuspendLayout();
            // 
            // flpCheckboxes
            // 
            this.flpCheckboxes.AccessibleDescription = null;
            this.flpCheckboxes.AccessibleName = null;
            resources.ApplyResources(this.flpCheckboxes, "flpCheckboxes");
            this.flpCheckboxes.BackgroundImage = null;
            this.flpCheckboxes.Controls.Add(this.cbxActive);
            this.flpCheckboxes.Controls.Add(this.btnCheckTrades);
            this.flpCheckboxes.Font = null;
            this.flpCheckboxes.Name = "flpCheckboxes";
            // 
            // cbxActive
            // 
            this.cbxActive.AccessibleDescription = null;
            this.cbxActive.AccessibleName = null;
            resources.ApplyResources(this.cbxActive, "cbxActive");
            this.cbxActive.BackgroundImage = null;
            this.cbxActive.Font = null;
            this.cbxActive.Name = "cbxActive";
            this.cbxActive.UseVisualStyleBackColor = true;
            this.cbxActive.CheckedChanged += new System.EventHandler(this.cbxActive_CheckedChanged);
            // 
            // btnCheckTrades
            // 
            this.btnCheckTrades.AccessibleDescription = null;
            this.btnCheckTrades.AccessibleName = null;
            resources.ApplyResources(this.btnCheckTrades, "btnCheckTrades");
            this.btnCheckTrades.BackgroundImage = null;
            this.btnCheckTrades.Font = null;
            this.btnCheckTrades.Name = "btnCheckTrades";
            this.btnCheckTrades.UseVisualStyleBackColor = true;
            this.btnCheckTrades.Click += new System.EventHandler(this.btnCheckTrades_Click);
            // 
            // dgvLTD
            // 
            this.dgvLTD.AccessibleDescription = null;
            this.dgvLTD.AccessibleName = null;
            this.dgvLTD.AllowUserToAddRows = false;
            this.dgvLTD.AllowUserToDeleteRows = false;
            resources.ApplyResources(this.dgvLTD, "dgvLTD");
            this.dgvLTD.BackgroundImage = null;
            this.dgvLTD.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLTD.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clhMatch,
            this.clhPlaytime,
            this.clhScore,
            this.clhWinBack,
            this.clhWinLay,
            this.clhCloseTradeTimer,
            this.clhOpenBetTimer,
            this.clhStopLossTimer,
            this.clhLocalConfig,
            this.clhLivescore1,
            this.clhLivescore2});
            this.dgvLTD.Font = null;
            this.dgvLTD.Name = "dgvLTD";
            this.dgvLTD.ReadOnly = true;
            this.dgvLTD.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvLTD_CellValueChanged);
            this.dgvLTD.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dgvLTD_RowsAdded);
            this.dgvLTD.CellMouseEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvLTD_CellMouseEnter);
            this.dgvLTD.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvLTD_CellContentClick);
            // 
            // clhMatch
            // 
            this.clhMatch.Frozen = true;
            resources.ApplyResources(this.clhMatch, "clhMatch");
            this.clhMatch.Name = "clhMatch";
            this.clhMatch.ReadOnly = true;
            // 
            // clhPlaytime
            // 
            this.clhPlaytime.Frozen = true;
            resources.ApplyResources(this.clhPlaytime, "clhPlaytime");
            this.clhPlaytime.Name = "clhPlaytime";
            this.clhPlaytime.ReadOnly = true;
            // 
            // clhScore
            // 
            this.clhScore.Frozen = true;
            resources.ApplyResources(this.clhScore, "clhScore");
            this.clhScore.Name = "clhScore";
            this.clhScore.ReadOnly = true;
            // 
            // clhWinBack
            // 
            this.clhWinBack.Frozen = true;
            resources.ApplyResources(this.clhWinBack, "clhWinBack");
            this.clhWinBack.Name = "clhWinBack";
            this.clhWinBack.ReadOnly = true;
            // 
            // clhWinLay
            // 
            this.clhWinLay.Frozen = true;
            resources.ApplyResources(this.clhWinLay, "clhWinLay");
            this.clhWinLay.Name = "clhWinLay";
            this.clhWinLay.ReadOnly = true;
            // 
            // clhCloseTradeTimer
            // 
            this.clhCloseTradeTimer.Frozen = true;
            resources.ApplyResources(this.clhCloseTradeTimer, "clhCloseTradeTimer");
            this.clhCloseTradeTimer.Name = "clhCloseTradeTimer";
            this.clhCloseTradeTimer.ReadOnly = true;
            // 
            // clhOpenBetTimer
            // 
            this.clhOpenBetTimer.Frozen = true;
            resources.ApplyResources(this.clhOpenBetTimer, "clhOpenBetTimer");
            this.clhOpenBetTimer.Name = "clhOpenBetTimer";
            this.clhOpenBetTimer.ReadOnly = true;
            // 
            // clhStopLossTimer
            // 
            this.clhStopLossTimer.Frozen = true;
            resources.ApplyResources(this.clhStopLossTimer, "clhStopLossTimer");
            this.clhStopLossTimer.Name = "clhStopLossTimer";
            this.clhStopLossTimer.ReadOnly = true;
            // 
            // clhLocalConfig
            // 
            this.clhLocalConfig.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.clhLocalConfig.Frozen = true;
            resources.ApplyResources(this.clhLocalConfig, "clhLocalConfig");
            this.clhLocalConfig.Name = "clhLocalConfig";
            this.clhLocalConfig.ReadOnly = true;
            this.clhLocalConfig.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // clhLivescore1
            // 
            resources.ApplyResources(this.clhLivescore1, "clhLivescore1");
            this.clhLivescore1.Name = "clhLivescore1";
            this.clhLivescore1.ReadOnly = true;
            // 
            // clhLivescore2
            // 
            resources.ApplyResources(this.clhLivescore2, "clhLivescore2");
            this.clhLivescore2.Name = "clhLivescore2";
            this.clhLivescore2.ReadOnly = true;
            // 
            // ctlLayDraw_v2
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.Controls.Add(this.dgvLTD);
            this.Controls.Add(this.flpCheckboxes);
            this.Font = null;
            this.Name = "ctlLayDraw_v2";
            this.flpCheckboxes.ResumeLayout(false);
            this.flpCheckboxes.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLTD)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flpCheckboxes;
        private System.Windows.Forms.CheckBox cbxActive;
        private System.Windows.Forms.DataGridView dgvLTD;
        private System.Windows.Forms.Button btnCheckTrades;
        private System.Windows.Forms.DataGridViewTextBoxColumn clhMatch;
        private System.Windows.Forms.DataGridViewTextBoxColumn clhPlaytime;
        private System.Windows.Forms.DataGridViewTextBoxColumn clhScore;
        private System.Windows.Forms.DataGridViewTextBoxColumn clhWinBack;
        private System.Windows.Forms.DataGridViewTextBoxColumn clhWinLay;
        private System.Windows.Forms.DataGridViewTextBoxColumn clhCloseTradeTimer;
        private System.Windows.Forms.DataGridViewTextBoxColumn clhOpenBetTimer;
        private System.Windows.Forms.DataGridViewTextBoxColumn clhStopLossTimer;
        private System.Windows.Forms.DataGridViewButtonColumn clhLocalConfig;
        private System.Windows.Forms.DataGridViewTextBoxColumn clhLivescore1;
        private System.Windows.Forms.DataGridViewTextBoxColumn clhLivescore2;
    }
}
