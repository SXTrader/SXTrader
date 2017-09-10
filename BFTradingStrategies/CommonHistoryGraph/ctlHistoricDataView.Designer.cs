namespace net.sxtrader.bftradingstrategies.common
{
    partial class ctlHistoricDataView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ctlHistoricDataView));
            this.gbxTitel = new System.Windows.Forms.GroupBox();
            this.pnlListView = new System.Windows.Forms.Panel();
            this.lvwHistoricData = new System.Windows.Forms.ListView();
            this.clhDevision = new System.Windows.Forms.ColumnHeader();
            this.clhDate = new System.Windows.Forms.ColumnHeader();
            this.clhTeamA = new System.Windows.Forms.ColumnHeader();
            this.clhScore = new System.Windows.Forms.ColumnHeader();
            this.clhTeamB = new System.Windows.Forms.ColumnHeader();
            this.pnlAdditional = new System.Windows.Forms.Panel();
            this.btnMore = new System.Windows.Forms.Button();
            this.lblZeroToZeroNumber = new System.Windows.Forms.Label();
            this.lblZeroToZero = new System.Windows.Forms.Label();
            this.lblLatestFirstGoalTimeNumber = new System.Windows.Forms.Label();
            this.lblLatestFirstGoalTime = new System.Windows.Forms.Label();
            this.lblEarlierstFirstGoalTimeNumber = new System.Windows.Forms.Label();
            this.lblEarlierstFirstGoalTime = new System.Windows.Forms.Label();
            this.lblAvgFirstGoalTimeNumber = new System.Windows.Forms.Label();
            this.lblAvgFirstGoalTime = new System.Windows.Forms.Label();
            this.lblAvgGoalNumber = new System.Windows.Forms.Label();
            this.lblAvgGoalText = new System.Windows.Forms.Label();
            this.gbxTitel.SuspendLayout();
            this.pnlListView.SuspendLayout();
            this.pnlAdditional.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbxTitel
            // 
            this.gbxTitel.AccessibleDescription = null;
            this.gbxTitel.AccessibleName = null;
            resources.ApplyResources(this.gbxTitel, "gbxTitel");
            this.gbxTitel.BackgroundImage = null;
            this.gbxTitel.Controls.Add(this.pnlListView);
            this.gbxTitel.Controls.Add(this.pnlAdditional);
            this.gbxTitel.Font = null;
            this.gbxTitel.Name = "gbxTitel";
            this.gbxTitel.TabStop = false;
            // 
            // pnlListView
            // 
            this.pnlListView.AccessibleDescription = null;
            this.pnlListView.AccessibleName = null;
            resources.ApplyResources(this.pnlListView, "pnlListView");
            this.pnlListView.BackgroundImage = null;
            this.pnlListView.Controls.Add(this.lvwHistoricData);
            this.pnlListView.Font = null;
            this.pnlListView.Name = "pnlListView";
            // 
            // lvwHistoricData
            // 
            this.lvwHistoricData.AccessibleDescription = null;
            this.lvwHistoricData.AccessibleName = null;
            resources.ApplyResources(this.lvwHistoricData, "lvwHistoricData");
            this.lvwHistoricData.BackgroundImage = null;
            this.lvwHistoricData.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clhDevision,
            this.clhDate,
            this.clhTeamA,
            this.clhScore,
            this.clhTeamB});
            this.lvwHistoricData.Font = null;
            this.lvwHistoricData.FullRowSelect = true;
            this.lvwHistoricData.GridLines = true;
            this.lvwHistoricData.MultiSelect = false;
            this.lvwHistoricData.Name = "lvwHistoricData";
            this.lvwHistoricData.UseCompatibleStateImageBehavior = false;
            this.lvwHistoricData.View = System.Windows.Forms.View.Details;
            this.lvwHistoricData.SelectedIndexChanged += new System.EventHandler(this.lvwHistoricData_SelectedIndexChanged);
            // 
            // clhDevision
            // 
            resources.ApplyResources(this.clhDevision, "clhDevision");
            // 
            // clhDate
            // 
            resources.ApplyResources(this.clhDate, "clhDate");
            // 
            // clhTeamA
            // 
            resources.ApplyResources(this.clhTeamA, "clhTeamA");
            // 
            // clhScore
            // 
            resources.ApplyResources(this.clhScore, "clhScore");
            // 
            // clhTeamB
            // 
            resources.ApplyResources(this.clhTeamB, "clhTeamB");
            // 
            // pnlAdditional
            // 
            this.pnlAdditional.AccessibleDescription = null;
            this.pnlAdditional.AccessibleName = null;
            resources.ApplyResources(this.pnlAdditional, "pnlAdditional");
            this.pnlAdditional.BackgroundImage = null;
            this.pnlAdditional.Controls.Add(this.btnMore);
            this.pnlAdditional.Controls.Add(this.lblZeroToZeroNumber);
            this.pnlAdditional.Controls.Add(this.lblZeroToZero);
            this.pnlAdditional.Controls.Add(this.lblLatestFirstGoalTimeNumber);
            this.pnlAdditional.Controls.Add(this.lblLatestFirstGoalTime);
            this.pnlAdditional.Controls.Add(this.lblEarlierstFirstGoalTimeNumber);
            this.pnlAdditional.Controls.Add(this.lblEarlierstFirstGoalTime);
            this.pnlAdditional.Controls.Add(this.lblAvgFirstGoalTimeNumber);
            this.pnlAdditional.Controls.Add(this.lblAvgFirstGoalTime);
            this.pnlAdditional.Controls.Add(this.lblAvgGoalNumber);
            this.pnlAdditional.Controls.Add(this.lblAvgGoalText);
            this.pnlAdditional.Font = null;
            this.pnlAdditional.Name = "pnlAdditional";
            // 
            // btnMore
            // 
            this.btnMore.AccessibleDescription = null;
            this.btnMore.AccessibleName = null;
            resources.ApplyResources(this.btnMore, "btnMore");
            this.btnMore.BackgroundImage = null;
            this.btnMore.Font = null;
            this.btnMore.Name = "btnMore";
            this.btnMore.UseVisualStyleBackColor = true;
            this.btnMore.Click += new System.EventHandler(this.btnMore_Click);
            // 
            // lblZeroToZeroNumber
            // 
            this.lblZeroToZeroNumber.AccessibleDescription = null;
            this.lblZeroToZeroNumber.AccessibleName = null;
            resources.ApplyResources(this.lblZeroToZeroNumber, "lblZeroToZeroNumber");
            this.lblZeroToZeroNumber.Font = null;
            this.lblZeroToZeroNumber.Name = "lblZeroToZeroNumber";
            // 
            // lblZeroToZero
            // 
            this.lblZeroToZero.AccessibleDescription = null;
            this.lblZeroToZero.AccessibleName = null;
            resources.ApplyResources(this.lblZeroToZero, "lblZeroToZero");
            this.lblZeroToZero.Font = null;
            this.lblZeroToZero.Name = "lblZeroToZero";
            // 
            // lblLatestFirstGoalTimeNumber
            // 
            this.lblLatestFirstGoalTimeNumber.AccessibleDescription = null;
            this.lblLatestFirstGoalTimeNumber.AccessibleName = null;
            resources.ApplyResources(this.lblLatestFirstGoalTimeNumber, "lblLatestFirstGoalTimeNumber");
            this.lblLatestFirstGoalTimeNumber.Font = null;
            this.lblLatestFirstGoalTimeNumber.Name = "lblLatestFirstGoalTimeNumber";
            // 
            // lblLatestFirstGoalTime
            // 
            this.lblLatestFirstGoalTime.AccessibleDescription = null;
            this.lblLatestFirstGoalTime.AccessibleName = null;
            resources.ApplyResources(this.lblLatestFirstGoalTime, "lblLatestFirstGoalTime");
            this.lblLatestFirstGoalTime.Font = null;
            this.lblLatestFirstGoalTime.Name = "lblLatestFirstGoalTime";
            // 
            // lblEarlierstFirstGoalTimeNumber
            // 
            this.lblEarlierstFirstGoalTimeNumber.AccessibleDescription = null;
            this.lblEarlierstFirstGoalTimeNumber.AccessibleName = null;
            resources.ApplyResources(this.lblEarlierstFirstGoalTimeNumber, "lblEarlierstFirstGoalTimeNumber");
            this.lblEarlierstFirstGoalTimeNumber.Font = null;
            this.lblEarlierstFirstGoalTimeNumber.Name = "lblEarlierstFirstGoalTimeNumber";
            // 
            // lblEarlierstFirstGoalTime
            // 
            this.lblEarlierstFirstGoalTime.AccessibleDescription = null;
            this.lblEarlierstFirstGoalTime.AccessibleName = null;
            resources.ApplyResources(this.lblEarlierstFirstGoalTime, "lblEarlierstFirstGoalTime");
            this.lblEarlierstFirstGoalTime.Font = null;
            this.lblEarlierstFirstGoalTime.Name = "lblEarlierstFirstGoalTime";
            // 
            // lblAvgFirstGoalTimeNumber
            // 
            this.lblAvgFirstGoalTimeNumber.AccessibleDescription = null;
            this.lblAvgFirstGoalTimeNumber.AccessibleName = null;
            resources.ApplyResources(this.lblAvgFirstGoalTimeNumber, "lblAvgFirstGoalTimeNumber");
            this.lblAvgFirstGoalTimeNumber.Font = null;
            this.lblAvgFirstGoalTimeNumber.Name = "lblAvgFirstGoalTimeNumber";
            // 
            // lblAvgFirstGoalTime
            // 
            this.lblAvgFirstGoalTime.AccessibleDescription = null;
            this.lblAvgFirstGoalTime.AccessibleName = null;
            resources.ApplyResources(this.lblAvgFirstGoalTime, "lblAvgFirstGoalTime");
            this.lblAvgFirstGoalTime.Font = null;
            this.lblAvgFirstGoalTime.Name = "lblAvgFirstGoalTime";
            // 
            // lblAvgGoalNumber
            // 
            this.lblAvgGoalNumber.AccessibleDescription = null;
            this.lblAvgGoalNumber.AccessibleName = null;
            resources.ApplyResources(this.lblAvgGoalNumber, "lblAvgGoalNumber");
            this.lblAvgGoalNumber.Font = null;
            this.lblAvgGoalNumber.Name = "lblAvgGoalNumber";
            // 
            // lblAvgGoalText
            // 
            this.lblAvgGoalText.AccessibleDescription = null;
            this.lblAvgGoalText.AccessibleName = null;
            resources.ApplyResources(this.lblAvgGoalText, "lblAvgGoalText");
            this.lblAvgGoalText.Font = null;
            this.lblAvgGoalText.Name = "lblAvgGoalText";
            // 
            // ctlHistoricDataView
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.Controls.Add(this.gbxTitel);
            this.Font = null;
            this.Name = "ctlHistoricDataView";
            this.gbxTitel.ResumeLayout(false);
            this.pnlListView.ResumeLayout(false);
            this.pnlAdditional.ResumeLayout(false);
            this.pnlAdditional.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbxTitel;
        private System.Windows.Forms.Panel pnlAdditional;
        private System.Windows.Forms.Panel pnlListView;
        private System.Windows.Forms.ListView lvwHistoricData;
        private System.Windows.Forms.ColumnHeader clhDevision;
        private System.Windows.Forms.ColumnHeader clhDate;
        private System.Windows.Forms.ColumnHeader clhTeamA;
        private System.Windows.Forms.ColumnHeader clhScore;
        private System.Windows.Forms.ColumnHeader clhTeamB;
        private System.Windows.Forms.Label lblAvgFirstGoalTimeNumber;
        private System.Windows.Forms.Label lblAvgFirstGoalTime;
        private System.Windows.Forms.Label lblAvgGoalNumber;
        private System.Windows.Forms.Label lblAvgGoalText;
        private System.Windows.Forms.Label lblLatestFirstGoalTimeNumber;
        private System.Windows.Forms.Label lblLatestFirstGoalTime;
        private System.Windows.Forms.Label lblEarlierstFirstGoalTimeNumber;
        private System.Windows.Forms.Label lblEarlierstFirstGoalTime;
        private System.Windows.Forms.Label lblZeroToZeroNumber;
        private System.Windows.Forms.Label lblZeroToZero;
        private System.Windows.Forms.Button btnMore;
    }
}
