namespace net.sxtrader.bftradingstrategies.BFTSGUI
{
    partial class ctlConfig
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ctlConfig));
            this.gbxGeneral = new System.Windows.Forms.GroupBox();
            this.gbxLogging = new System.Windows.Forms.GroupBox();
            this.cbxLogBetAmounts = new System.Windows.Forms.CheckBox();
            this.cbxLogTrades = new System.Windows.Forms.CheckBox();
            this.cbxLogLiveticker = new System.Windows.Forms.CheckBox();
            this.cbxConfirmFastBet = new System.Windows.Forms.CheckBox();
            this.cbxWriteDebug = new System.Windows.Forms.CheckBox();
            this.lblBFSeconds = new System.Windows.Forms.Label();
            this.spnBFSeconds = new System.Windows.Forms.NumericUpDown();
            this.lblCheckBF = new System.Windows.Forms.Label();
            this.lblLiveSeconds = new System.Windows.Forms.Label();
            this.spnLiveSeconds = new System.Windows.Forms.NumericUpDown();
            this.lblLiveticker = new System.Windows.Forms.Label();
            this.gbxGeneral.SuspendLayout();
            this.gbxLogging.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spnBFSeconds)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnLiveSeconds)).BeginInit();
            this.SuspendLayout();
            // 
            // gbxGeneral
            // 
            this.gbxGeneral.Controls.Add(this.gbxLogging);
            this.gbxGeneral.Controls.Add(this.cbxConfirmFastBet);
            this.gbxGeneral.Controls.Add(this.cbxWriteDebug);
            this.gbxGeneral.Controls.Add(this.lblBFSeconds);
            this.gbxGeneral.Controls.Add(this.spnBFSeconds);
            this.gbxGeneral.Controls.Add(this.lblCheckBF);
            this.gbxGeneral.Controls.Add(this.lblLiveSeconds);
            this.gbxGeneral.Controls.Add(this.spnLiveSeconds);
            this.gbxGeneral.Controls.Add(this.lblLiveticker);
            resources.ApplyResources(this.gbxGeneral, "gbxGeneral");
            this.gbxGeneral.Name = "gbxGeneral";
            this.gbxGeneral.TabStop = false;
            // 
            // gbxLogging
            // 
            this.gbxLogging.Controls.Add(this.cbxLogBetAmounts);
            this.gbxLogging.Controls.Add(this.cbxLogTrades);
            this.gbxLogging.Controls.Add(this.cbxLogLiveticker);
            resources.ApplyResources(this.gbxLogging, "gbxLogging");
            this.gbxLogging.Name = "gbxLogging";
            this.gbxLogging.TabStop = false;
            // 
            // cbxLogBetAmounts
            // 
            resources.ApplyResources(this.cbxLogBetAmounts, "cbxLogBetAmounts");
            this.cbxLogBetAmounts.Name = "cbxLogBetAmounts";
            this.cbxLogBetAmounts.UseVisualStyleBackColor = true;
            // 
            // cbxLogTrades
            // 
            resources.ApplyResources(this.cbxLogTrades, "cbxLogTrades");
            this.cbxLogTrades.Name = "cbxLogTrades";
            this.cbxLogTrades.UseVisualStyleBackColor = true;
            // 
            // cbxLogLiveticker
            // 
            resources.ApplyResources(this.cbxLogLiveticker, "cbxLogLiveticker");
            this.cbxLogLiveticker.Name = "cbxLogLiveticker";
            this.cbxLogLiveticker.UseVisualStyleBackColor = true;
            // 
            // cbxConfirmFastBet
            // 
            resources.ApplyResources(this.cbxConfirmFastBet, "cbxConfirmFastBet");
            this.cbxConfirmFastBet.Name = "cbxConfirmFastBet";
            this.cbxConfirmFastBet.UseVisualStyleBackColor = true;
            // 
            // cbxWriteDebug
            // 
            resources.ApplyResources(this.cbxWriteDebug, "cbxWriteDebug");
            this.cbxWriteDebug.Name = "cbxWriteDebug";
            this.cbxWriteDebug.UseVisualStyleBackColor = true;
            // 
            // lblBFSeconds
            // 
            resources.ApplyResources(this.lblBFSeconds, "lblBFSeconds");
            this.lblBFSeconds.Name = "lblBFSeconds";
            // 
            // spnBFSeconds
            // 
            resources.ApplyResources(this.spnBFSeconds, "spnBFSeconds");
            this.spnBFSeconds.Maximum = new decimal(new int[] {
            1200,
            0,
            0,
            0});
            this.spnBFSeconds.Name = "spnBFSeconds";
            // 
            // lblCheckBF
            // 
            resources.ApplyResources(this.lblCheckBF, "lblCheckBF");
            this.lblCheckBF.Name = "lblCheckBF";
            // 
            // lblLiveSeconds
            // 
            resources.ApplyResources(this.lblLiveSeconds, "lblLiveSeconds");
            this.lblLiveSeconds.Name = "lblLiveSeconds";
            // 
            // spnLiveSeconds
            // 
            resources.ApplyResources(this.spnLiveSeconds, "spnLiveSeconds");
            this.spnLiveSeconds.Maximum = new decimal(new int[] {
            600,
            0,
            0,
            0});
            this.spnLiveSeconds.Name = "spnLiveSeconds";
            // 
            // lblLiveticker
            // 
            resources.ApplyResources(this.lblLiveticker, "lblLiveticker");
            this.lblLiveticker.Name = "lblLiveticker";
            // 
            // ctlConfig
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gbxGeneral);
            this.Name = "ctlConfig";
            this.gbxGeneral.ResumeLayout(false);
            this.gbxGeneral.PerformLayout();
            this.gbxLogging.ResumeLayout(false);
            this.gbxLogging.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spnBFSeconds)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnLiveSeconds)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbxGeneral;
        private System.Windows.Forms.Label lblLiveticker;
        private System.Windows.Forms.NumericUpDown spnLiveSeconds;
        private System.Windows.Forms.Label lblLiveSeconds;
        private System.Windows.Forms.NumericUpDown spnBFSeconds;
        private System.Windows.Forms.Label lblCheckBF;
        private System.Windows.Forms.Label lblBFSeconds;
        private System.Windows.Forms.CheckBox cbxWriteDebug;
        private System.Windows.Forms.CheckBox cbxConfirmFastBet;
        private System.Windows.Forms.GroupBox gbxLogging;
        private System.Windows.Forms.CheckBox cbxLogLiveticker;
        private System.Windows.Forms.CheckBox cbxLogTrades;
        private System.Windows.Forms.CheckBox cbxLogBetAmounts;

    }
}
