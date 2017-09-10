namespace net.sxtrader.bftradingstrategies.ttr.GUI
{
    partial class ctlTTR
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ctlTTR));
            this.pnlGUI = new System.Windows.Forms.SplitContainer();
            this.dgvTrades = new System.Windows.Forms.DataGridView();
            this.flpDetailView = new System.Windows.Forms.FlowLayoutPanel();
            this.clhMatch = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clhTradeType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clhPlaytime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clhScore = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clhTradeState = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clhCountDown = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clhWin1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clhWin2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clhCustomizing = new System.Windows.Forms.DataGridViewButtonColumn();
            this.pnlGUI.Panel1.SuspendLayout();
            this.pnlGUI.Panel2.SuspendLayout();
            this.pnlGUI.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTrades)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlGUI
            // 
            resources.ApplyResources(this.pnlGUI, "pnlGUI");
            this.pnlGUI.Name = "pnlGUI";
            // 
            // pnlGUI.Panel1
            // 
            this.pnlGUI.Panel1.Controls.Add(this.dgvTrades);
            // 
            // pnlGUI.Panel2
            // 
            this.pnlGUI.Panel2.Controls.Add(this.flpDetailView);
            // 
            // dgvTrades
            // 
            this.dgvTrades.AllowUserToAddRows = false;
            this.dgvTrades.AllowUserToDeleteRows = false;
            this.dgvTrades.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTrades.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clhMatch,
            this.clhTradeType,
            this.clhPlaytime,
            this.clhScore,
            this.clhTradeState,
            this.clhCountDown,
            this.clhWin1,
            this.clhWin2,
            this.clhCustomizing});
            resources.ApplyResources(this.dgvTrades, "dgvTrades");
            this.dgvTrades.MultiSelect = false;
            this.dgvTrades.Name = "dgvTrades";
            this.dgvTrades.ReadOnly = true;
            this.dgvTrades.SelectionChanged += new System.EventHandler(this.dgvTrades_SelectionChanged);
            this.dgvTrades.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvTrades_CellContentClick);
            // 
            // flpDetailView
            // 
            resources.ApplyResources(this.flpDetailView, "flpDetailView");
            this.flpDetailView.Name = "flpDetailView";
            // 
            // clhMatch
            // 
            resources.ApplyResources(this.clhMatch, "clhMatch");
            this.clhMatch.Name = "clhMatch";
            this.clhMatch.ReadOnly = true;
            // 
            // clhTradeType
            // 
            resources.ApplyResources(this.clhTradeType, "clhTradeType");
            this.clhTradeType.Name = "clhTradeType";
            this.clhTradeType.ReadOnly = true;
            // 
            // clhPlaytime
            // 
            resources.ApplyResources(this.clhPlaytime, "clhPlaytime");
            this.clhPlaytime.Name = "clhPlaytime";
            this.clhPlaytime.ReadOnly = true;
            // 
            // clhScore
            // 
            resources.ApplyResources(this.clhScore, "clhScore");
            this.clhScore.Name = "clhScore";
            this.clhScore.ReadOnly = true;
            // 
            // clhTradeState
            // 
            resources.ApplyResources(this.clhTradeState, "clhTradeState");
            this.clhTradeState.Name = "clhTradeState";
            this.clhTradeState.ReadOnly = true;
            // 
            // clhCountDown
            // 
            resources.ApplyResources(this.clhCountDown, "clhCountDown");
            this.clhCountDown.Name = "clhCountDown";
            this.clhCountDown.ReadOnly = true;
            // 
            // clhWin1
            // 
            resources.ApplyResources(this.clhWin1, "clhWin1");
            this.clhWin1.Name = "clhWin1";
            this.clhWin1.ReadOnly = true;
            // 
            // clhWin2
            // 
            resources.ApplyResources(this.clhWin2, "clhWin2");
            this.clhWin2.Name = "clhWin2";
            this.clhWin2.ReadOnly = true;
            // 
            // clhCustomizing
            // 
            resources.ApplyResources(this.clhCustomizing, "clhCustomizing");
            this.clhCustomizing.Name = "clhCustomizing";
            this.clhCustomizing.ReadOnly = true;
            // 
            // ctlTTR
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlGUI);
            this.Name = "ctlTTR";
            this.pnlGUI.Panel1.ResumeLayout(false);
            this.pnlGUI.Panel2.ResumeLayout(false);
            this.pnlGUI.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTrades)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer pnlGUI;
        private System.Windows.Forms.DataGridView dgvTrades;
        private System.Windows.Forms.FlowLayoutPanel flpDetailView;
        private System.Windows.Forms.DataGridViewTextBoxColumn clhMatch;
        private System.Windows.Forms.DataGridViewTextBoxColumn clhTradeType;
        private System.Windows.Forms.DataGridViewTextBoxColumn clhPlaytime;
        private System.Windows.Forms.DataGridViewTextBoxColumn clhScore;
        private System.Windows.Forms.DataGridViewTextBoxColumn clhTradeState;
        private System.Windows.Forms.DataGridViewTextBoxColumn clhCountDown;
        private System.Windows.Forms.DataGridViewTextBoxColumn clhWin1;
        private System.Windows.Forms.DataGridViewTextBoxColumn clhWin2;
        private System.Windows.Forms.DataGridViewButtonColumn clhCustomizing;
    }
}
