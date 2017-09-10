namespace net.sxtrader.bftradingstrategies.ttr.TradeModuls.CorrectScore.Controls
{
    partial class ctlCSConfiguration
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ctlCSConfiguration));
            this.gbxSettings = new System.Windows.Forms.GroupBox();
            this.chkNoTrade = new System.Windows.Forms.CheckBox();
            this.chkUseGreenWaittime = new System.Windows.Forms.CheckBox();
            this.chkUseHedgeWaittime = new System.Windows.Forms.CheckBox();
            this.gbxPlaytime = new System.Windows.Forms.GroupBox();
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
            this.gbxGreen.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spnGreenDelta)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnGreenPlaytime)).BeginInit();
            this.gbxHedge.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spnHedgeDelta)).BeginInit();
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
            this.gbxSettings.AccessibleDescription = null;
            this.gbxSettings.AccessibleName = null;
            resources.ApplyResources(this.gbxSettings, "gbxSettings");
            this.gbxSettings.BackgroundImage = null;
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
            this.gbxSettings.Font = null;
            this.gbxSettings.Name = "gbxSettings";
            this.gbxSettings.TabStop = false;
            // 
            // chkNoTrade
            // 
            this.chkNoTrade.AccessibleDescription = null;
            this.chkNoTrade.AccessibleName = null;
            resources.ApplyResources(this.chkNoTrade, "chkNoTrade");
            this.chkNoTrade.BackgroundImage = null;
            this.chkNoTrade.Name = "chkNoTrade";
            this.chkNoTrade.UseVisualStyleBackColor = true;
            this.chkNoTrade.CheckedChanged += new System.EventHandler(this.chkNoTrade_CheckedChanged);
            // 
            // chkUseGreenWaittime
            // 
            this.chkUseGreenWaittime.AccessibleDescription = null;
            this.chkUseGreenWaittime.AccessibleName = null;
            resources.ApplyResources(this.chkUseGreenWaittime, "chkUseGreenWaittime");
            this.chkUseGreenWaittime.BackgroundImage = null;
            this.chkUseGreenWaittime.Font = null;
            this.chkUseGreenWaittime.Name = "chkUseGreenWaittime";
            this.chkUseGreenWaittime.UseVisualStyleBackColor = true;
            // 
            // chkUseHedgeWaittime
            // 
            this.chkUseHedgeWaittime.AccessibleDescription = null;
            this.chkUseHedgeWaittime.AccessibleName = null;
            resources.ApplyResources(this.chkUseHedgeWaittime, "chkUseHedgeWaittime");
            this.chkUseHedgeWaittime.BackgroundImage = null;
            this.chkUseHedgeWaittime.Font = null;
            this.chkUseHedgeWaittime.Name = "chkUseHedgeWaittime";
            this.chkUseHedgeWaittime.UseVisualStyleBackColor = true;
            // 
            // gbxPlaytime
            // 
            this.gbxPlaytime.AccessibleDescription = null;
            this.gbxPlaytime.AccessibleName = null;
            resources.ApplyResources(this.gbxPlaytime, "gbxPlaytime");
            this.gbxPlaytime.BackgroundImage = null;
            this.gbxPlaytime.Controls.Add(this.gbxGreen);
            this.gbxPlaytime.Controls.Add(this.gbxHedge);
            this.gbxPlaytime.Font = null;
            this.gbxPlaytime.Name = "gbxPlaytime";
            this.gbxPlaytime.TabStop = false;
            // 
            // gbxGreen
            // 
            this.gbxGreen.AccessibleDescription = null;
            this.gbxGreen.AccessibleName = null;
            resources.ApplyResources(this.gbxGreen, "gbxGreen");
            this.gbxGreen.BackgroundImage = null;
            this.gbxGreen.Controls.Add(this.spnGreenDelta);
            this.gbxGreen.Controls.Add(this.lblGreenDelta);
            this.gbxGreen.Controls.Add(this.cbxSpecialGreenPT);
            this.gbxGreen.Controls.Add(this.spnGreenPlaytime);
            this.gbxGreen.Controls.Add(this.rbnGreenDynamic);
            this.gbxGreen.Controls.Add(this.rbnFixedGreenPT);
            this.gbxGreen.Font = null;
            this.gbxGreen.Name = "gbxGreen";
            this.gbxGreen.TabStop = false;
            // 
            // spnGreenDelta
            // 
            this.spnGreenDelta.AccessibleDescription = null;
            this.spnGreenDelta.AccessibleName = null;
            resources.ApplyResources(this.spnGreenDelta, "spnGreenDelta");
            this.spnGreenDelta.Font = null;
            this.spnGreenDelta.Maximum = new decimal(new int[] {
            90,
            0,
            0,
            0});
            this.spnGreenDelta.Name = "spnGreenDelta";
            // 
            // lblGreenDelta
            // 
            this.lblGreenDelta.AccessibleDescription = null;
            this.lblGreenDelta.AccessibleName = null;
            resources.ApplyResources(this.lblGreenDelta, "lblGreenDelta");
            this.lblGreenDelta.Font = null;
            this.lblGreenDelta.Name = "lblGreenDelta";
            // 
            // cbxSpecialGreenPT
            // 
            this.cbxSpecialGreenPT.AccessibleDescription = null;
            this.cbxSpecialGreenPT.AccessibleName = null;
            resources.ApplyResources(this.cbxSpecialGreenPT, "cbxSpecialGreenPT");
            this.cbxSpecialGreenPT.BackgroundImage = null;
            this.cbxSpecialGreenPT.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxSpecialGreenPT.Font = null;
            this.cbxSpecialGreenPT.FormattingEnabled = true;
            this.cbxSpecialGreenPT.Name = "cbxSpecialGreenPT";
            // 
            // spnGreenPlaytime
            // 
            this.spnGreenPlaytime.AccessibleDescription = null;
            this.spnGreenPlaytime.AccessibleName = null;
            resources.ApplyResources(this.spnGreenPlaytime, "spnGreenPlaytime");
            this.spnGreenPlaytime.Font = null;
            this.spnGreenPlaytime.Maximum = new decimal(new int[] {
            90,
            0,
            0,
            0});
            this.spnGreenPlaytime.Name = "spnGreenPlaytime";
            // 
            // rbnGreenDynamic
            // 
            this.rbnGreenDynamic.AccessibleDescription = null;
            this.rbnGreenDynamic.AccessibleName = null;
            resources.ApplyResources(this.rbnGreenDynamic, "rbnGreenDynamic");
            this.rbnGreenDynamic.BackgroundImage = null;
            this.rbnGreenDynamic.Font = null;
            this.rbnGreenDynamic.Name = "rbnGreenDynamic";
            this.rbnGreenDynamic.TabStop = true;
            this.rbnGreenDynamic.UseVisualStyleBackColor = true;
            // 
            // rbnFixedGreenPT
            // 
            this.rbnFixedGreenPT.AccessibleDescription = null;
            this.rbnFixedGreenPT.AccessibleName = null;
            resources.ApplyResources(this.rbnFixedGreenPT, "rbnFixedGreenPT");
            this.rbnFixedGreenPT.BackgroundImage = null;
            this.rbnFixedGreenPT.Font = null;
            this.rbnFixedGreenPT.Name = "rbnFixedGreenPT";
            this.rbnFixedGreenPT.TabStop = true;
            this.rbnFixedGreenPT.UseVisualStyleBackColor = true;
            // 
            // gbxHedge
            // 
            this.gbxHedge.AccessibleDescription = null;
            this.gbxHedge.AccessibleName = null;
            resources.ApplyResources(this.gbxHedge, "gbxHedge");
            this.gbxHedge.BackgroundImage = null;
            this.gbxHedge.Controls.Add(this.spnHedgeDelta);
            this.gbxHedge.Controls.Add(this.lblHedgeDelta);
            this.gbxHedge.Controls.Add(this.cbxSpecialHedgePT);
            this.gbxHedge.Controls.Add(this.spnHedgePlaytime);
            this.gbxHedge.Controls.Add(this.rbnHedgeDynamic);
            this.gbxHedge.Controls.Add(this.rbnFixedHedgePT);
            this.gbxHedge.Font = null;
            this.gbxHedge.Name = "gbxHedge";
            this.gbxHedge.TabStop = false;
            // 
            // spnHedgeDelta
            // 
            this.spnHedgeDelta.AccessibleDescription = null;
            this.spnHedgeDelta.AccessibleName = null;
            resources.ApplyResources(this.spnHedgeDelta, "spnHedgeDelta");
            this.spnHedgeDelta.Font = null;
            this.spnHedgeDelta.Maximum = new decimal(new int[] {
            90,
            0,
            0,
            0});
            this.spnHedgeDelta.Name = "spnHedgeDelta";
            // 
            // lblHedgeDelta
            // 
            this.lblHedgeDelta.AccessibleDescription = null;
            this.lblHedgeDelta.AccessibleName = null;
            resources.ApplyResources(this.lblHedgeDelta, "lblHedgeDelta");
            this.lblHedgeDelta.Font = null;
            this.lblHedgeDelta.Name = "lblHedgeDelta";
            // 
            // cbxSpecialHedgePT
            // 
            this.cbxSpecialHedgePT.AccessibleDescription = null;
            this.cbxSpecialHedgePT.AccessibleName = null;
            resources.ApplyResources(this.cbxSpecialHedgePT, "cbxSpecialHedgePT");
            this.cbxSpecialHedgePT.BackgroundImage = null;
            this.cbxSpecialHedgePT.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxSpecialHedgePT.Font = null;
            this.cbxSpecialHedgePT.FormattingEnabled = true;
            this.cbxSpecialHedgePT.Name = "cbxSpecialHedgePT";
            // 
            // spnHedgePlaytime
            // 
            this.spnHedgePlaytime.AccessibleDescription = null;
            this.spnHedgePlaytime.AccessibleName = null;
            resources.ApplyResources(this.spnHedgePlaytime, "spnHedgePlaytime");
            this.spnHedgePlaytime.Font = null;
            this.spnHedgePlaytime.Maximum = new decimal(new int[] {
            90,
            0,
            0,
            0});
            this.spnHedgePlaytime.Name = "spnHedgePlaytime";
            // 
            // rbnHedgeDynamic
            // 
            this.rbnHedgeDynamic.AccessibleDescription = null;
            this.rbnHedgeDynamic.AccessibleName = null;
            resources.ApplyResources(this.rbnHedgeDynamic, "rbnHedgeDynamic");
            this.rbnHedgeDynamic.BackgroundImage = null;
            this.rbnHedgeDynamic.Font = null;
            this.rbnHedgeDynamic.Name = "rbnHedgeDynamic";
            this.rbnHedgeDynamic.TabStop = true;
            this.rbnHedgeDynamic.UseVisualStyleBackColor = true;
            // 
            // rbnFixedHedgePT
            // 
            this.rbnFixedHedgePT.AccessibleDescription = null;
            this.rbnFixedHedgePT.AccessibleName = null;
            resources.ApplyResources(this.rbnFixedHedgePT, "rbnFixedHedgePT");
            this.rbnFixedHedgePT.BackgroundImage = null;
            this.rbnFixedHedgePT.Font = null;
            this.rbnFixedHedgePT.Name = "rbnFixedHedgePT";
            this.rbnFixedHedgePT.TabStop = true;
            this.rbnFixedHedgePT.UseVisualStyleBackColor = true;
            // 
            // gbxOddsPercentage
            // 
            this.gbxOddsPercentage.AccessibleDescription = null;
            this.gbxOddsPercentage.AccessibleName = null;
            resources.ApplyResources(this.gbxOddsPercentage, "gbxOddsPercentage");
            this.gbxOddsPercentage.BackgroundImage = null;
            this.gbxOddsPercentage.Controls.Add(this.spnGreenPercentage);
            this.gbxOddsPercentage.Controls.Add(this.lblGreenPercentage);
            this.gbxOddsPercentage.Controls.Add(this.spnHedgePercentage);
            this.gbxOddsPercentage.Controls.Add(this.lblHedgePercentage);
            this.gbxOddsPercentage.Font = null;
            this.gbxOddsPercentage.Name = "gbxOddsPercentage";
            this.gbxOddsPercentage.TabStop = false;
            // 
            // spnGreenPercentage
            // 
            this.spnGreenPercentage.AccessibleDescription = null;
            this.spnGreenPercentage.AccessibleName = null;
            resources.ApplyResources(this.spnGreenPercentage, "spnGreenPercentage");
            this.spnGreenPercentage.Font = null;
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
            this.lblGreenPercentage.AccessibleDescription = null;
            this.lblGreenPercentage.AccessibleName = null;
            resources.ApplyResources(this.lblGreenPercentage, "lblGreenPercentage");
            this.lblGreenPercentage.Font = null;
            this.lblGreenPercentage.Name = "lblGreenPercentage";
            // 
            // spnHedgePercentage
            // 
            this.spnHedgePercentage.AccessibleDescription = null;
            this.spnHedgePercentage.AccessibleName = null;
            resources.ApplyResources(this.spnHedgePercentage, "spnHedgePercentage");
            this.spnHedgePercentage.Font = null;
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
            this.lblHedgePercentage.AccessibleDescription = null;
            this.lblHedgePercentage.AccessibleName = null;
            resources.ApplyResources(this.lblHedgePercentage, "lblHedgePercentage");
            this.lblHedgePercentage.Font = null;
            this.lblHedgePercentage.Name = "lblHedgePercentage";
            // 
            // gbxWaitTimes
            // 
            this.gbxWaitTimes.AccessibleDescription = null;
            this.gbxWaitTimes.AccessibleName = null;
            resources.ApplyResources(this.gbxWaitTimes, "gbxWaitTimes");
            this.gbxWaitTimes.BackgroundImage = null;
            this.gbxWaitTimes.Controls.Add(this.spnGreenWaitTime);
            this.gbxWaitTimes.Controls.Add(this.lblGreenWaitTime);
            this.gbxWaitTimes.Controls.Add(this.spnHedgeWaitTime);
            this.gbxWaitTimes.Controls.Add(this.lblHedgeWaitTime);
            this.gbxWaitTimes.Font = null;
            this.gbxWaitTimes.Name = "gbxWaitTimes";
            this.gbxWaitTimes.TabStop = false;
            // 
            // spnGreenWaitTime
            // 
            this.spnGreenWaitTime.AccessibleDescription = null;
            this.spnGreenWaitTime.AccessibleName = null;
            resources.ApplyResources(this.spnGreenWaitTime, "spnGreenWaitTime");
            this.spnGreenWaitTime.Font = null;
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
            this.lblGreenWaitTime.AccessibleDescription = null;
            this.lblGreenWaitTime.AccessibleName = null;
            resources.ApplyResources(this.lblGreenWaitTime, "lblGreenWaitTime");
            this.lblGreenWaitTime.Font = null;
            this.lblGreenWaitTime.Name = "lblGreenWaitTime";
            // 
            // spnHedgeWaitTime
            // 
            this.spnHedgeWaitTime.AccessibleDescription = null;
            this.spnHedgeWaitTime.AccessibleName = null;
            resources.ApplyResources(this.spnHedgeWaitTime, "spnHedgeWaitTime");
            this.spnHedgeWaitTime.Font = null;
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
            this.lblHedgeWaitTime.AccessibleDescription = null;
            this.lblHedgeWaitTime.AccessibleName = null;
            resources.ApplyResources(this.lblHedgeWaitTime, "lblHedgeWaitTime");
            this.lblHedgeWaitTime.Font = null;
            this.lblHedgeWaitTime.Name = "lblHedgeWaitTime";
            // 
            // chkOddsPercentage
            // 
            this.chkOddsPercentage.AccessibleDescription = null;
            this.chkOddsPercentage.AccessibleName = null;
            resources.ApplyResources(this.chkOddsPercentage, "chkOddsPercentage");
            this.chkOddsPercentage.BackgroundImage = null;
            this.chkOddsPercentage.Font = null;
            this.chkOddsPercentage.Name = "chkOddsPercentage";
            this.chkOddsPercentage.UseVisualStyleBackColor = true;
            // 
            // chkWaitTimes
            // 
            this.chkWaitTimes.AccessibleDescription = null;
            this.chkWaitTimes.AccessibleName = null;
            resources.ApplyResources(this.chkWaitTimes, "chkWaitTimes");
            this.chkWaitTimes.BackgroundImage = null;
            this.chkWaitTimes.Font = null;
            this.chkWaitTimes.Name = "chkWaitTimes";
            this.chkWaitTimes.UseVisualStyleBackColor = true;
            // 
            // chkCheckLayOdds
            // 
            this.chkCheckLayOdds.AccessibleDescription = null;
            this.chkCheckLayOdds.AccessibleName = null;
            resources.ApplyResources(this.chkCheckLayOdds, "chkCheckLayOdds");
            this.chkCheckLayOdds.BackgroundImage = null;
            this.chkCheckLayOdds.Font = null;
            this.chkCheckLayOdds.Name = "chkCheckLayOdds";
            this.chkCheckLayOdds.UseVisualStyleBackColor = true;
            // 
            // chkOnlyHedge
            // 
            this.chkOnlyHedge.AccessibleDescription = null;
            this.chkOnlyHedge.AccessibleName = null;
            resources.ApplyResources(this.chkOnlyHedge, "chkOnlyHedge");
            this.chkOnlyHedge.BackgroundImage = null;
            this.chkOnlyHedge.Font = null;
            this.chkOnlyHedge.Name = "chkOnlyHedge";
            this.chkOnlyHedge.UseVisualStyleBackColor = true;
            // 
            // ctlCSConfiguration
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.Controls.Add(this.gbxSettings);
            this.Font = null;
            this.Name = "ctlCSConfiguration";
            this.SizeChanged += new System.EventHandler(this.ctlCSConfiguration_SizeChanged);
            this.gbxSettings.ResumeLayout(false);
            this.gbxSettings.PerformLayout();
            this.gbxPlaytime.ResumeLayout(false);
            this.gbxGreen.ResumeLayout(false);
            this.gbxGreen.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spnGreenDelta)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnGreenPlaytime)).EndInit();
            this.gbxHedge.ResumeLayout(false);
            this.gbxHedge.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spnHedgeDelta)).EndInit();
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
        private System.Windows.Forms.GroupBox gbxGreen;
        private System.Windows.Forms.NumericUpDown spnGreenDelta;
        private System.Windows.Forms.Label lblGreenDelta;
        private System.Windows.Forms.ComboBox cbxSpecialGreenPT;
        private System.Windows.Forms.NumericUpDown spnGreenPlaytime;
        private System.Windows.Forms.RadioButton rbnGreenDynamic;
        private System.Windows.Forms.RadioButton rbnFixedGreenPT;
        private System.Windows.Forms.GroupBox gbxHedge;
        private System.Windows.Forms.NumericUpDown spnHedgeDelta;
        private System.Windows.Forms.Label lblHedgeDelta;
        private System.Windows.Forms.ComboBox cbxSpecialHedgePT;
        private System.Windows.Forms.NumericUpDown spnHedgePlaytime;
        private System.Windows.Forms.RadioButton rbnHedgeDynamic;
        private System.Windows.Forms.RadioButton rbnFixedHedgePT;
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
