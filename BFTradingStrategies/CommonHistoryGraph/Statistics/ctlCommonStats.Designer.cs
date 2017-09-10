namespace net.sxtrader.bftradingstrategies.common.Statistics
{
    partial class ctlCommonStats
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ctlCommonStats));
            this.lblAvgGoals = new System.Windows.Forms.Label();
            this.lblAvg1stGoal = new System.Windows.Forms.Label();
            this.lblEarliest1stGoal = new System.Windows.Forms.Label();
            this.lblLatest1stGoal = new System.Windows.Forms.Label();
            this.lblAvgGoalNumber = new System.Windows.Forms.Label();
            this.lblAvg1stGoalNumber = new System.Windows.Forms.Label();
            this.lblEarlist1stGoalNumber = new System.Windows.Forms.Label();
            this.lblLatest1stGoalNumber = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblAvgGoals
            // 
            resources.ApplyResources(this.lblAvgGoals, "lblAvgGoals");
            this.lblAvgGoals.Name = "lblAvgGoals";
            // 
            // lblAvg1stGoal
            // 
            resources.ApplyResources(this.lblAvg1stGoal, "lblAvg1stGoal");
            this.lblAvg1stGoal.Name = "lblAvg1stGoal";
            // 
            // lblEarliest1stGoal
            // 
            resources.ApplyResources(this.lblEarliest1stGoal, "lblEarliest1stGoal");
            this.lblEarliest1stGoal.Name = "lblEarliest1stGoal";
            // 
            // lblLatest1stGoal
            // 
            resources.ApplyResources(this.lblLatest1stGoal, "lblLatest1stGoal");
            this.lblLatest1stGoal.Name = "lblLatest1stGoal";
            // 
            // lblAvgGoalNumber
            // 
            resources.ApplyResources(this.lblAvgGoalNumber, "lblAvgGoalNumber");
            this.lblAvgGoalNumber.Name = "lblAvgGoalNumber";
            // 
            // lblAvg1stGoalNumber
            // 
            resources.ApplyResources(this.lblAvg1stGoalNumber, "lblAvg1stGoalNumber");
            this.lblAvg1stGoalNumber.Name = "lblAvg1stGoalNumber";
            // 
            // lblEarlist1stGoalNumber
            // 
            resources.ApplyResources(this.lblEarlist1stGoalNumber, "lblEarlist1stGoalNumber");
            this.lblEarlist1stGoalNumber.Name = "lblEarlist1stGoalNumber";
            // 
            // lblLatest1stGoalNumber
            // 
            resources.ApplyResources(this.lblLatest1stGoalNumber, "lblLatest1stGoalNumber");
            this.lblLatest1stGoalNumber.Name = "lblLatest1stGoalNumber";
            // 
            // ctlCommonStats
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblLatest1stGoalNumber);
            this.Controls.Add(this.lblEarlist1stGoalNumber);
            this.Controls.Add(this.lblAvg1stGoalNumber);
            this.Controls.Add(this.lblAvgGoalNumber);
            this.Controls.Add(this.lblLatest1stGoal);
            this.Controls.Add(this.lblEarliest1stGoal);
            this.Controls.Add(this.lblAvg1stGoal);
            this.Controls.Add(this.lblAvgGoals);
            this.Name = "ctlCommonStats";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblAvgGoals;
        private System.Windows.Forms.Label lblAvg1stGoal;
        private System.Windows.Forms.Label lblEarliest1stGoal;
        private System.Windows.Forms.Label lblLatest1stGoal;
        private System.Windows.Forms.Label lblAvgGoalNumber;
        private System.Windows.Forms.Label lblAvg1stGoalNumber;
        private System.Windows.Forms.Label lblEarlist1stGoalNumber;
        private System.Windows.Forms.Label lblLatest1stGoalNumber;
    }
}
