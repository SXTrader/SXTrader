namespace net.sxtrader.bftradingstrategies.common
{
    partial class ctlStatisticAnalyses
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
            this.pnlSplit = new System.Windows.Forms.SplitContainer();
            this.lblButtons = new System.Windows.Forms.Label();
            this.pnlSplitRight = new System.Windows.Forms.SplitContainer();
            this.pnlSplitStatistics = new System.Windows.Forms.SplitContainer();
            this.pnlSplitStats = new System.Windows.Forms.SplitContainer();
            this.pnlMatchDetail = new System.Windows.Forms.Panel();
            this.lblDetails = new System.Windows.Forms.Label();
            this.pnlBets = new System.Windows.Forms.Panel();
            this.lblBetModuls = new System.Windows.Forms.Label();
            this.pnlSplitStatsTop = new System.Windows.Forms.SplitContainer();
            ((System.ComponentModel.ISupportInitialize)(this.pnlSplit)).BeginInit();
            this.pnlSplit.Panel1.SuspendLayout();
            this.pnlSplit.Panel2.SuspendLayout();
            this.pnlSplit.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pnlSplitRight)).BeginInit();
            this.pnlSplitRight.Panel1.SuspendLayout();
            this.pnlSplitRight.Panel2.SuspendLayout();
            this.pnlSplitRight.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pnlSplitStatistics)).BeginInit();
            this.pnlSplitStatistics.Panel1.SuspendLayout();
            this.pnlSplitStatistics.Panel2.SuspendLayout();
            this.pnlSplitStatistics.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pnlSplitStats)).BeginInit();
            this.pnlSplitStats.Panel1.SuspendLayout();
            this.pnlSplitStats.SuspendLayout();
            this.pnlMatchDetail.SuspendLayout();
            this.pnlBets.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pnlSplitStatsTop)).BeginInit();
            this.pnlSplitStatsTop.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlSplit
            // 
            this.pnlSplit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlSplit.Location = new System.Drawing.Point(0, 0);
            this.pnlSplit.Name = "pnlSplit";
            // 
            // pnlSplit.Panel1
            // 
            this.pnlSplit.Panel1.AutoScroll = true;
            this.pnlSplit.Panel1.BackColor = System.Drawing.SystemColors.Info;
            this.pnlSplit.Panel1.Controls.Add(this.lblButtons);
            // 
            // pnlSplit.Panel2
            // 
            this.pnlSplit.Panel2.Controls.Add(this.pnlSplitRight);
            this.pnlSplit.Size = new System.Drawing.Size(909, 615);
            this.pnlSplit.SplitterDistance = 303;
            this.pnlSplit.TabIndex = 0;
            // 
            // lblButtons
            // 
            this.lblButtons.AutoSize = true;
            this.lblButtons.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblButtons.Location = new System.Drawing.Point(51, 295);
            this.lblButtons.Name = "lblButtons";
            this.lblButtons.Size = new System.Drawing.Size(201, 25);
            this.lblButtons.TabIndex = 0;
            this.lblButtons.Text = "No Inplay markets";
            // 
            // pnlSplitRight
            // 
            this.pnlSplitRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlSplitRight.Location = new System.Drawing.Point(0, 0);
            this.pnlSplitRight.Name = "pnlSplitRight";
            this.pnlSplitRight.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // pnlSplitRight.Panel1
            // 
            this.pnlSplitRight.Panel1.Controls.Add(this.pnlSplitStatistics);
            // 
            // pnlSplitRight.Panel2
            // 
            this.pnlSplitRight.Panel2.Controls.Add(this.pnlBets);
            this.pnlSplitRight.Size = new System.Drawing.Size(602, 615);
            this.pnlSplitRight.SplitterDistance = 362;
            this.pnlSplitRight.TabIndex = 0;
            // 
            // pnlSplitStatistics
            // 
            this.pnlSplitStatistics.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlSplitStatistics.Location = new System.Drawing.Point(0, 0);
            this.pnlSplitStatistics.Name = "pnlSplitStatistics";
            // 
            // pnlSplitStatistics.Panel1
            // 
            this.pnlSplitStatistics.Panel1.Controls.Add(this.pnlSplitStats);
            // 
            // pnlSplitStatistics.Panel2
            // 
            this.pnlSplitStatistics.Panel2.Controls.Add(this.pnlMatchDetail);
            this.pnlSplitStatistics.Size = new System.Drawing.Size(602, 362);
            this.pnlSplitStatistics.SplitterDistance = 225;
            this.pnlSplitStatistics.TabIndex = 0;
            // 
            // pnlSplitStats
            // 
            this.pnlSplitStats.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlSplitStats.Location = new System.Drawing.Point(0, 0);
            this.pnlSplitStats.Name = "pnlSplitStats";
            this.pnlSplitStats.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // pnlSplitStats.Panel1
            // 
            this.pnlSplitStats.Panel1.Controls.Add(this.pnlSplitStatsTop);
            this.pnlSplitStats.Size = new System.Drawing.Size(225, 362);
            this.pnlSplitStats.SplitterDistance = 262;
            this.pnlSplitStats.TabIndex = 0;
            this.pnlSplitStats.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.pnlSplitStats_SplitterMoved);
            // 
            // pnlMatchDetail
            // 
            this.pnlMatchDetail.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.pnlMatchDetail.Controls.Add(this.lblDetails);
            this.pnlMatchDetail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMatchDetail.Location = new System.Drawing.Point(0, 0);
            this.pnlMatchDetail.Name = "pnlMatchDetail";
            this.pnlMatchDetail.Size = new System.Drawing.Size(373, 362);
            this.pnlMatchDetail.TabIndex = 9;
            // 
            // lblDetails
            // 
            this.lblDetails.AutoSize = true;
            this.lblDetails.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDetails.Location = new System.Drawing.Point(40, 195);
            this.lblDetails.Name = "lblDetails";
            this.lblDetails.Size = new System.Drawing.Size(121, 25);
            this.lblDetails.TabIndex = 1;
            this.lblDetails.Text = "No Details";
            // 
            // pnlBets
            // 
            this.pnlBets.BackColor = System.Drawing.Color.Blue;
            this.pnlBets.Controls.Add(this.lblBetModuls);
            this.pnlBets.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlBets.Location = new System.Drawing.Point(0, 0);
            this.pnlBets.Name = "pnlBets";
            this.pnlBets.Size = new System.Drawing.Size(602, 249);
            this.pnlBets.TabIndex = 6;
            // 
            // lblBetModuls
            // 
            this.lblBetModuls.AutoSize = true;
            this.lblBetModuls.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBetModuls.Location = new System.Drawing.Point(218, 88);
            this.lblBetModuls.Name = "lblBetModuls";
            this.lblBetModuls.Size = new System.Drawing.Size(166, 25);
            this.lblBetModuls.TabIndex = 1;
            this.lblBetModuls.Text = "No Bet Moduls";
            // 
            // pnlSplitStatsTop
            // 
            this.pnlSplitStatsTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlSplitStatsTop.Location = new System.Drawing.Point(0, 0);
            this.pnlSplitStatsTop.Name = "pnlSplitStatsTop";
            this.pnlSplitStatsTop.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.pnlSplitStatsTop.Size = new System.Drawing.Size(225, 262);
            this.pnlSplitStatsTop.SplitterDistance = 127;
            this.pnlSplitStatsTop.TabIndex = 0;
            // 
            // ctlStatisticAnalyses
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlSplit);
            this.Name = "ctlStatisticAnalyses";
            this.Size = new System.Drawing.Size(909, 615);
            this.Load += new System.EventHandler(this.ctlStatisticAnalyses_Load);
            this.DockChanged += new System.EventHandler(this.ctlStatisticAnalyses_DockChanged);
            this.EnabledChanged += new System.EventHandler(this.ctlStatisticAnalyses_EnabledChanged);
            this.pnlSplit.Panel1.ResumeLayout(false);
            this.pnlSplit.Panel1.PerformLayout();
            this.pnlSplit.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pnlSplit)).EndInit();
            this.pnlSplit.ResumeLayout(false);
            this.pnlSplitRight.Panel1.ResumeLayout(false);
            this.pnlSplitRight.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pnlSplitRight)).EndInit();
            this.pnlSplitRight.ResumeLayout(false);
            this.pnlSplitStatistics.Panel1.ResumeLayout(false);
            this.pnlSplitStatistics.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pnlSplitStatistics)).EndInit();
            this.pnlSplitStatistics.ResumeLayout(false);
            this.pnlSplitStats.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pnlSplitStats)).EndInit();
            this.pnlSplitStats.ResumeLayout(false);
            this.pnlMatchDetail.ResumeLayout(false);
            this.pnlMatchDetail.PerformLayout();
            this.pnlBets.ResumeLayout(false);
            this.pnlBets.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pnlSplitStatsTop)).EndInit();
            this.pnlSplitStatsTop.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer pnlSplit;
        private System.Windows.Forms.Label lblButtons;
        private System.Windows.Forms.SplitContainer pnlSplitRight;
        private System.Windows.Forms.Panel pnlBets;
        private System.Windows.Forms.Label lblBetModuls;
        private System.Windows.Forms.SplitContainer pnlSplitStatistics;
        private System.Windows.Forms.Panel pnlMatchDetail;
        private System.Windows.Forms.Label lblDetails;
        private System.Windows.Forms.SplitContainer pnlSplitStats;
        private System.Windows.Forms.SplitContainer pnlSplitStatsTop;
    }
}
