namespace net.sxtrader.bftradingstrategies.ttr.GUI.Configuration
{
    partial class ctlSL00Configuration
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ctlSL00Configuration));
            this.chkOnlyHedge = new System.Windows.Forms.CheckBox();
            this.chkCheckLayOdds = new System.Windows.Forms.CheckBox();
            this.chkWaitTimes = new System.Windows.Forms.CheckBox();
            this.chkOddsPercentage = new System.Windows.Forms.CheckBox();
            this.gbxWaitTimes = new System.Windows.Forms.GroupBox();
            this.spnGreenWaitTime = new System.Windows.Forms.NumericUpDown();
            this.lblGreenWaitTime = new System.Windows.Forms.Label();
            this.spnHedgeWaitTime = new System.Windows.Forms.NumericUpDown();
            this.lblHedgeWaitTime = new System.Windows.Forms.Label();
            this.gbxOddsPercentage = new System.Windows.Forms.GroupBox();
            this.spnGreenPercentage = new System.Windows.Forms.NumericUpDown();
            this.lblGreenPercentage = new System.Windows.Forms.Label();
            this.spnHedgePercentage = new System.Windows.Forms.NumericUpDown();
            this.lblHedgePercentage = new System.Windows.Forms.Label();
            this.chkNoTrade = new System.Windows.Forms.CheckBox();
            this.chkUseGreenWaittime = new System.Windows.Forms.CheckBox();
            this.chkUseHedgeWaittime = new System.Windows.Forms.CheckBox();
            this.gbxPlaytimes = new System.Windows.Forms.GroupBox();
            this.gbxGreen = new System.Windows.Forms.GroupBox();
            this.spnGreenDelta = new System.Windows.Forms.NumericUpDown();
            this.lblGreenDelta = new System.Windows.Forms.Label();
            this.cbxSpecialGreenPT = new System.Windows.Forms.ComboBox();
            this.spnGreenPlaytime = new System.Windows.Forms.NumericUpDown();
            this.rbnGreenDynamic = new System.Windows.Forms.RadioButton();
            this.rbnFixedGreenPT = new System.Windows.Forms.RadioButton();
            this.gbxHedge = new System.Windows.Forms.GroupBox();
            this.spnHedgeDelta = new System.Windows.Forms.NumericUpDown();
            this.lblHedgeDelta = new System.Windows.Forms.Label();
            this.cbxSpecialHedgePT = new System.Windows.Forms.ComboBox();
            this.spnHedgePlaytime = new System.Windows.Forms.NumericUpDown();
            this.rbnHedgeDynamic = new System.Windows.Forms.RadioButton();
            this.rbnFixedHedgePT = new System.Windows.Forms.RadioButton();
            this.gbxWaitTimes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spnGreenWaitTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnHedgeWaitTime)).BeginInit();
            this.gbxOddsPercentage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spnGreenPercentage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnHedgePercentage)).BeginInit();
            this.gbxPlaytimes.SuspendLayout();
            this.gbxGreen.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spnGreenDelta)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnGreenPlaytime)).BeginInit();
            this.gbxHedge.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spnHedgeDelta)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnHedgePlaytime)).BeginInit();
            this.SuspendLayout();
            // 
            // chkOnlyHedge
            // 
            resources.ApplyResources(this.chkOnlyHedge, "chkOnlyHedge");
            this.chkOnlyHedge.Name = "chkOnlyHedge";
            this.chkOnlyHedge.UseVisualStyleBackColor = true;
            // 
            // chkCheckLayOdds
            // 
            resources.ApplyResources(this.chkCheckLayOdds, "chkCheckLayOdds");
            this.chkCheckLayOdds.Name = "chkCheckLayOdds";
            this.chkCheckLayOdds.UseVisualStyleBackColor = true;
            // 
            // chkWaitTimes
            // 
            resources.ApplyResources(this.chkWaitTimes, "chkWaitTimes");
            this.chkWaitTimes.Name = "chkWaitTimes";
            this.chkWaitTimes.UseVisualStyleBackColor = true;
            // 
            // chkOddsPercentage
            // 
            resources.ApplyResources(this.chkOddsPercentage, "chkOddsPercentage");
            this.chkOddsPercentage.Name = "chkOddsPercentage";
            this.chkOddsPercentage.UseVisualStyleBackColor = true;
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
            // gbxPlaytimes
            // 
            this.gbxPlaytimes.Controls.Add(this.gbxGreen);
            this.gbxPlaytimes.Controls.Add(this.gbxHedge);
            resources.ApplyResources(this.gbxPlaytimes, "gbxPlaytimes");
            this.gbxPlaytimes.Name = "gbxPlaytimes";
            this.gbxPlaytimes.TabStop = false;
            // 
            // gbxGreen
            // 
            this.gbxGreen.Controls.Add(this.spnGreenDelta);
            this.gbxGreen.Controls.Add(this.lblGreenDelta);
            this.gbxGreen.Controls.Add(this.cbxSpecialGreenPT);
            this.gbxGreen.Controls.Add(this.spnGreenPlaytime);
            this.gbxGreen.Controls.Add(this.rbnGreenDynamic);
            this.gbxGreen.Controls.Add(this.rbnFixedGreenPT);
            resources.ApplyResources(this.gbxGreen, "gbxGreen");
            this.gbxGreen.Name = "gbxGreen";
            this.gbxGreen.TabStop = false;
            // 
            // spnGreenDelta
            // 
            resources.ApplyResources(this.spnGreenDelta, "spnGreenDelta");
            this.spnGreenDelta.Maximum = new decimal(new int[] {
            90,
            0,
            0,
            0});
            this.spnGreenDelta.Name = "spnGreenDelta";
            // 
            // lblGreenDelta
            // 
            resources.ApplyResources(this.lblGreenDelta, "lblGreenDelta");
            this.lblGreenDelta.Name = "lblGreenDelta";
            // 
            // cbxSpecialGreenPT
            // 
            this.cbxSpecialGreenPT.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxSpecialGreenPT.FormattingEnabled = true;
            resources.ApplyResources(this.cbxSpecialGreenPT, "cbxSpecialGreenPT");
            this.cbxSpecialGreenPT.Name = "cbxSpecialGreenPT";
            // 
            // spnGreenPlaytime
            // 
            resources.ApplyResources(this.spnGreenPlaytime, "spnGreenPlaytime");
            this.spnGreenPlaytime.Maximum = new decimal(new int[] {
            90,
            0,
            0,
            0});
            this.spnGreenPlaytime.Name = "spnGreenPlaytime";
            // 
            // rbnGreenDynamic
            // 
            resources.ApplyResources(this.rbnGreenDynamic, "rbnGreenDynamic");
            this.rbnGreenDynamic.Name = "rbnGreenDynamic";
            this.rbnGreenDynamic.TabStop = true;
            this.rbnGreenDynamic.UseVisualStyleBackColor = true;
            // 
            // rbnFixedGreenPT
            // 
            resources.ApplyResources(this.rbnFixedGreenPT, "rbnFixedGreenPT");
            this.rbnFixedGreenPT.Name = "rbnFixedGreenPT";
            this.rbnFixedGreenPT.TabStop = true;
            this.rbnFixedGreenPT.UseVisualStyleBackColor = true;
            // 
            // gbxHedge
            // 
            this.gbxHedge.Controls.Add(this.spnHedgeDelta);
            this.gbxHedge.Controls.Add(this.lblHedgeDelta);
            this.gbxHedge.Controls.Add(this.cbxSpecialHedgePT);
            this.gbxHedge.Controls.Add(this.spnHedgePlaytime);
            this.gbxHedge.Controls.Add(this.rbnHedgeDynamic);
            this.gbxHedge.Controls.Add(this.rbnFixedHedgePT);
            resources.ApplyResources(this.gbxHedge, "gbxHedge");
            this.gbxHedge.Name = "gbxHedge";
            this.gbxHedge.TabStop = false;
            // 
            // spnHedgeDelta
            // 
            resources.ApplyResources(this.spnHedgeDelta, "spnHedgeDelta");
            this.spnHedgeDelta.Maximum = new decimal(new int[] {
            90,
            0,
            0,
            0});
            this.spnHedgeDelta.Name = "spnHedgeDelta";
            // 
            // lblHedgeDelta
            // 
            resources.ApplyResources(this.lblHedgeDelta, "lblHedgeDelta");
            this.lblHedgeDelta.Name = "lblHedgeDelta";
            // 
            // cbxSpecialHedgePT
            // 
            this.cbxSpecialHedgePT.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxSpecialHedgePT.FormattingEnabled = true;
            resources.ApplyResources(this.cbxSpecialHedgePT, "cbxSpecialHedgePT");
            this.cbxSpecialHedgePT.Name = "cbxSpecialHedgePT";
            // 
            // spnHedgePlaytime
            // 
            resources.ApplyResources(this.spnHedgePlaytime, "spnHedgePlaytime");
            this.spnHedgePlaytime.Maximum = new decimal(new int[] {
            90,
            0,
            0,
            0});
            this.spnHedgePlaytime.Name = "spnHedgePlaytime";
            // 
            // rbnHedgeDynamic
            // 
            resources.ApplyResources(this.rbnHedgeDynamic, "rbnHedgeDynamic");
            this.rbnHedgeDynamic.Name = "rbnHedgeDynamic";
            this.rbnHedgeDynamic.TabStop = true;
            this.rbnHedgeDynamic.UseVisualStyleBackColor = true;
            // 
            // rbnFixedHedgePT
            // 
            resources.ApplyResources(this.rbnFixedHedgePT, "rbnFixedHedgePT");
            this.rbnFixedHedgePT.Name = "rbnFixedHedgePT";
            this.rbnFixedHedgePT.TabStop = true;
            this.rbnFixedHedgePT.UseVisualStyleBackColor = true;
            // 
            // ctlSL00Configuration
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gbxPlaytimes);
            this.Controls.Add(this.chkUseGreenWaittime);
            this.Controls.Add(this.chkUseHedgeWaittime);
            this.Controls.Add(this.chkNoTrade);
            this.Controls.Add(this.gbxOddsPercentage);
            this.Controls.Add(this.gbxWaitTimes);
            this.Controls.Add(this.chkOddsPercentage);
            this.Controls.Add(this.chkWaitTimes);
            this.Controls.Add(this.chkCheckLayOdds);
            this.Controls.Add(this.chkOnlyHedge);
            this.Name = "ctlSL00Configuration";
            this.SizeChanged += new System.EventHandler(this.ctlSL00Configuration_SizeChanged);
            this.gbxWaitTimes.ResumeLayout(false);
            this.gbxWaitTimes.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spnGreenWaitTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnHedgeWaitTime)).EndInit();
            this.gbxOddsPercentage.ResumeLayout(false);
            this.gbxOddsPercentage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spnGreenPercentage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnHedgePercentage)).EndInit();
            this.gbxPlaytimes.ResumeLayout(false);
            this.gbxGreen.ResumeLayout(false);
            this.gbxGreen.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spnGreenDelta)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnGreenPlaytime)).EndInit();
            this.gbxHedge.ResumeLayout(false);
            this.gbxHedge.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spnHedgeDelta)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnHedgePlaytime)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkOnlyHedge;
        private System.Windows.Forms.CheckBox chkCheckLayOdds;
        private System.Windows.Forms.CheckBox chkWaitTimes;
        private System.Windows.Forms.CheckBox chkOddsPercentage;
        private System.Windows.Forms.GroupBox gbxWaitTimes;
        private System.Windows.Forms.Label lblHedgeWaitTime;
        private System.Windows.Forms.NumericUpDown spnHedgeWaitTime;
        private System.Windows.Forms.Label lblGreenWaitTime;
        private System.Windows.Forms.NumericUpDown spnGreenWaitTime;
        private System.Windows.Forms.GroupBox gbxOddsPercentage;
        private System.Windows.Forms.Label lblHedgePercentage;
        private System.Windows.Forms.NumericUpDown spnHedgePercentage;
        private System.Windows.Forms.NumericUpDown spnGreenPercentage;
        private System.Windows.Forms.Label lblGreenPercentage;
        private System.Windows.Forms.CheckBox chkNoTrade;
        private System.Windows.Forms.CheckBox chkUseGreenWaittime;
        private System.Windows.Forms.CheckBox chkUseHedgeWaittime;
        private System.Windows.Forms.GroupBox gbxPlaytimes;
        private System.Windows.Forms.GroupBox gbxHedge;
        private System.Windows.Forms.RadioButton rbnHedgeDynamic;
        private System.Windows.Forms.RadioButton rbnFixedHedgePT;
        private System.Windows.Forms.NumericUpDown spnHedgePlaytime;
        private System.Windows.Forms.ComboBox cbxSpecialHedgePT;
        private System.Windows.Forms.GroupBox gbxGreen;
        private System.Windows.Forms.NumericUpDown spnGreenDelta;
        private System.Windows.Forms.Label lblGreenDelta;
        private System.Windows.Forms.ComboBox cbxSpecialGreenPT;
        private System.Windows.Forms.NumericUpDown spnGreenPlaytime;
        private System.Windows.Forms.RadioButton rbnGreenDynamic;
        private System.Windows.Forms.RadioButton rbnFixedGreenPT;
        private System.Windows.Forms.NumericUpDown spnHedgeDelta;
        private System.Windows.Forms.Label lblHedgeDelta;
    }
}
