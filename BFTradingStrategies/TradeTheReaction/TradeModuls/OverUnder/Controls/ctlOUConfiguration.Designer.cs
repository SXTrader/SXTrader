namespace net.sxtrader.bftradingstrategies.ttr.TradeModuls.OverUnder.Controls
{
    partial class ctlOUConfiguration
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ctlOUConfiguration));
            this.gbxSettings = new System.Windows.Forms.GroupBox();
            this.chkNoTrade = new System.Windows.Forms.CheckBox();
            this.chkUseGreenWaittime = new System.Windows.Forms.CheckBox();
            this.chkUseHedgeWaittime = new System.Windows.Forms.CheckBox();
            this.gbxPlaytime = new System.Windows.Forms.GroupBox();
            this.spnGreenPlaytime = new System.Windows.Forms.NumericUpDown();
            this.lblGreenPlaytime = new System.Windows.Forms.Label();
            this.spnHedgePlaytime = new System.Windows.Forms.NumericUpDown();
            this.lblHedgePlaytime = new System.Windows.Forms.Label();
            this.gbxOddsPercentage = new System.Windows.Forms.GroupBox();
            this.spnGreenPercentage = new System.Windows.Forms.NumericUpDown();
            this.lblGreenPercentage = new System.Windows.Forms.Label();
            this.spnHedgePercentage = new System.Windows.Forms.NumericUpDown();
            this.lblHedgePercentage = new System.Windows.Forms.Label();
            this.gbxWaitTimes = new System.Windows.Forms.GroupBox();
            this.spnGreenWaitTime = new System.Windows.Forms.NumericUpDown();
            this.lblGreenWaitTime = new System.Windows.Forms.Label();
            this.spnHedgeWaitTime = new System.Windows.Forms.NumericUpDown();
            this.lblHedgeWaitTime = new System.Windows.Forms.Label();
            this.chkOddsPercentage = new System.Windows.Forms.CheckBox();
            this.chkWaitTimes = new System.Windows.Forms.CheckBox();
            this.chkCheckLayOdds = new System.Windows.Forms.CheckBox();
            this.chkOnlyHedge = new System.Windows.Forms.CheckBox();
            this.gbxSettings.SuspendLayout();
            this.gbxPlaytime.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spnGreenPlaytime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnHedgePlaytime)).BeginInit();
            this.gbxOddsPercentage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spnGreenPercentage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnHedgePercentage)).BeginInit();
            this.gbxWaitTimes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spnGreenWaitTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnHedgeWaitTime)).BeginInit();
            this.SuspendLayout();
            // 
            // gbxSettings
            // 
            this.gbxSettings.Controls.Add(this.chkNoTrade);
            this.gbxSettings.Controls.Add(this.chkUseGreenWaittime);
            this.gbxSettings.Controls.Add(this.chkUseHedgeWaittime);
            this.gbxSettings.Controls.Add(this.gbxPlaytime);
            this.gbxSettings.Controls.Add(this.gbxOddsPercentage);
            this.gbxSettings.Controls.Add(this.gbxWaitTimes);
            this.gbxSettings.Controls.Add(this.chkOddsPercentage);
            this.gbxSettings.Controls.Add(this.chkWaitTimes);
            this.gbxSettings.Controls.Add(this.chkCheckLayOdds);
            this.gbxSettings.Controls.Add(this.chkOnlyHedge);
            resources.ApplyResources(this.gbxSettings, "gbxSettings");
            this.gbxSettings.Name = "gbxSettings";
            this.gbxSettings.TabStop = false;
            // 
            // chkNoTrade
            // 
            resources.ApplyResources(this.chkNoTrade, "chkNoTrade");
            this.chkNoTrade.Name = "chkNoTrade";
            this.chkNoTrade.UseVisualStyleBackColor = true;
            this.chkNoTrade.CheckedChanged += new System.EventHandler(this.chkNoTrade_CheckedChanged);
            // 
            // chkUseGreenWaittime
            // 
            resources.ApplyResources(this.chkUseGreenWaittime, "chkUseGreenWaittime");
            this.chkUseGreenWaittime.Name = "chkUseGreenWaittime";
            this.chkUseGreenWaittime.UseVisualStyleBackColor = true;
            // 
            // chkUseHedgeWaittime
            // 
            resources.ApplyResources(this.chkUseHedgeWaittime, "chkUseHedgeWaittime");
            this.chkUseHedgeWaittime.Name = "chkUseHedgeWaittime";
            this.chkUseHedgeWaittime.UseVisualStyleBackColor = true;
            // 
            // gbxPlaytime
            // 
            this.gbxPlaytime.Controls.Add(this.spnGreenPlaytime);
            this.gbxPlaytime.Controls.Add(this.lblGreenPlaytime);
            this.gbxPlaytime.Controls.Add(this.spnHedgePlaytime);
            this.gbxPlaytime.Controls.Add(this.lblHedgePlaytime);
            resources.ApplyResources(this.gbxPlaytime, "gbxPlaytime");
            this.gbxPlaytime.Name = "gbxPlaytime";
            this.gbxPlaytime.TabStop = false;
            // 
            // spnGreenPlaytime
            // 
            resources.ApplyResources(this.spnGreenPlaytime, "spnGreenPlaytime");
            this.spnGreenPlaytime.Maximum = new decimal(new int[] {
            90,
            0,
            0,
            0});
            this.spnGreenPlaytime.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.spnGreenPlaytime.Name = "spnGreenPlaytime";
            this.spnGreenPlaytime.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lblGreenPlaytime
            // 
            resources.ApplyResources(this.lblGreenPlaytime, "lblGreenPlaytime");
            this.lblGreenPlaytime.Name = "lblGreenPlaytime";
            // 
            // spnHedgePlaytime
            // 
            resources.ApplyResources(this.spnHedgePlaytime, "spnHedgePlaytime");
            this.spnHedgePlaytime.Maximum = new decimal(new int[] {
            90,
            0,
            0,
            0});
            this.spnHedgePlaytime.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.spnHedgePlaytime.Name = "spnHedgePlaytime";
            this.spnHedgePlaytime.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lblHedgePlaytime
            // 
            resources.ApplyResources(this.lblHedgePlaytime, "lblHedgePlaytime");
            this.lblHedgePlaytime.Name = "lblHedgePlaytime";
            // 
            // gbxOddsPercentage
            // 
            this.gbxOddsPercentage.Controls.Add(this.spnGreenPercentage);
            this.gbxOddsPercentage.Controls.Add(this.lblGreenPercentage);
            this.gbxOddsPercentage.Controls.Add(this.spnHedgePercentage);
            this.gbxOddsPercentage.Controls.Add(this.lblHedgePercentage);
            resources.ApplyResources(this.gbxOddsPercentage, "gbxOddsPercentage");
            this.gbxOddsPercentage.Name = "gbxOddsPercentage";
            this.gbxOddsPercentage.TabStop = false;
            // 
            // spnGreenPercentage
            // 
            resources.ApplyResources(this.spnGreenPercentage, "spnGreenPercentage");
            this.spnGreenPercentage.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.spnGreenPercentage.Name = "spnGreenPercentage";
            this.spnGreenPercentage.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lblGreenPercentage
            // 
            resources.ApplyResources(this.lblGreenPercentage, "lblGreenPercentage");
            this.lblGreenPercentage.Name = "lblGreenPercentage";
            // 
            // spnHedgePercentage
            // 
            resources.ApplyResources(this.spnHedgePercentage, "spnHedgePercentage");
            this.spnHedgePercentage.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.spnHedgePercentage.Name = "spnHedgePercentage";
            this.spnHedgePercentage.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lblHedgePercentage
            // 
            resources.ApplyResources(this.lblHedgePercentage, "lblHedgePercentage");
            this.lblHedgePercentage.Name = "lblHedgePercentage";
            // 
            // gbxWaitTimes
            // 
            this.gbxWaitTimes.Controls.Add(this.spnGreenWaitTime);
            this.gbxWaitTimes.Controls.Add(this.lblGreenWaitTime);
            this.gbxWaitTimes.Controls.Add(this.spnHedgeWaitTime);
            this.gbxWaitTimes.Controls.Add(this.lblHedgeWaitTime);
            resources.ApplyResources(this.gbxWaitTimes, "gbxWaitTimes");
            this.gbxWaitTimes.Name = "gbxWaitTimes";
            this.gbxWaitTimes.TabStop = false;
            // 
            // spnGreenWaitTime
            // 
            resources.ApplyResources(this.spnGreenWaitTime, "spnGreenWaitTime");
            this.spnGreenWaitTime.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.spnGreenWaitTime.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.spnGreenWaitTime.Name = "spnGreenWaitTime";
            this.spnGreenWaitTime.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lblGreenWaitTime
            // 
            resources.ApplyResources(this.lblGreenWaitTime, "lblGreenWaitTime");
            this.lblGreenWaitTime.Name = "lblGreenWaitTime";
            // 
            // spnHedgeWaitTime
            // 
            resources.ApplyResources(this.spnHedgeWaitTime, "spnHedgeWaitTime");
            this.spnHedgeWaitTime.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.spnHedgeWaitTime.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.spnHedgeWaitTime.Name = "spnHedgeWaitTime";
            this.spnHedgeWaitTime.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lblHedgeWaitTime
            // 
            resources.ApplyResources(this.lblHedgeWaitTime, "lblHedgeWaitTime");
            this.lblHedgeWaitTime.Name = "lblHedgeWaitTime";
            // 
            // chkOddsPercentage
            // 
            resources.ApplyResources(this.chkOddsPercentage, "chkOddsPercentage");
            this.chkOddsPercentage.Name = "chkOddsPercentage";
            this.chkOddsPercentage.UseVisualStyleBackColor = true;
            // 
            // chkWaitTimes
            // 
            resources.ApplyResources(this.chkWaitTimes, "chkWaitTimes");
            this.chkWaitTimes.Name = "chkWaitTimes";
            this.chkWaitTimes.UseVisualStyleBackColor = true;
            // 
            // chkCheckLayOdds
            // 
            resources.ApplyResources(this.chkCheckLayOdds, "chkCheckLayOdds");
            this.chkCheckLayOdds.Name = "chkCheckLayOdds";
            this.chkCheckLayOdds.UseVisualStyleBackColor = true;
            // 
            // chkOnlyHedge
            // 
            resources.ApplyResources(this.chkOnlyHedge, "chkOnlyHedge");
            this.chkOnlyHedge.Name = "chkOnlyHedge";
            this.chkOnlyHedge.UseVisualStyleBackColor = true;
            // 
            // ctlOUConfiguration
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gbxSettings);
            this.Name = "ctlOUConfiguration";
            this.gbxSettings.ResumeLayout(false);
            this.gbxSettings.PerformLayout();
            this.gbxPlaytime.ResumeLayout(false);
            this.gbxPlaytime.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spnGreenPlaytime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnHedgePlaytime)).EndInit();
            this.gbxOddsPercentage.ResumeLayout(false);
            this.gbxOddsPercentage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spnGreenPercentage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnHedgePercentage)).EndInit();
            this.gbxWaitTimes.ResumeLayout(false);
            this.gbxWaitTimes.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spnGreenWaitTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnHedgeWaitTime)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbxSettings;
        private System.Windows.Forms.CheckBox chkNoTrade;
        private System.Windows.Forms.CheckBox chkUseGreenWaittime;
        private System.Windows.Forms.CheckBox chkUseHedgeWaittime;
        private System.Windows.Forms.GroupBox gbxPlaytime;
        private System.Windows.Forms.NumericUpDown spnGreenPlaytime;
        private System.Windows.Forms.Label lblGreenPlaytime;
        private System.Windows.Forms.NumericUpDown spnHedgePlaytime;
        private System.Windows.Forms.Label lblHedgePlaytime;
        private System.Windows.Forms.GroupBox gbxOddsPercentage;
        private System.Windows.Forms.NumericUpDown spnGreenPercentage;
        private System.Windows.Forms.Label lblGreenPercentage;
        private System.Windows.Forms.NumericUpDown spnHedgePercentage;
        private System.Windows.Forms.Label lblHedgePercentage;
        private System.Windows.Forms.GroupBox gbxWaitTimes;
        private System.Windows.Forms.NumericUpDown spnGreenWaitTime;
        private System.Windows.Forms.Label lblGreenWaitTime;
        private System.Windows.Forms.NumericUpDown spnHedgeWaitTime;
        private System.Windows.Forms.Label lblHedgeWaitTime;
        private System.Windows.Forms.CheckBox chkOddsPercentage;
        private System.Windows.Forms.CheckBox chkWaitTimes;
        private System.Windows.Forms.CheckBox chkCheckLayOdds;
        private System.Windows.Forms.CheckBox chkOnlyHedge;
    }
}
