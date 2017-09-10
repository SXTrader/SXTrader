namespace net.sxtrader.bftradingstrategies.ttr.GUI.Configuration
{
    partial class ctlTradeOutRuleEditOver
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ctlTradeOutRuleEditOver));
            this.pnlButtons = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnNew = new System.Windows.Forms.Button();
            this.gbxTrigger = new System.Windows.Forms.GroupBox();
            this.spnCheckOrder = new System.Windows.Forms.NumericUpDown();
            this.lblCheckOder = new System.Windows.Forms.Label();
            this.cbxTrigger = new System.Windows.Forms.ComboBox();
            this.lblTrigger = new System.Windows.Forms.Label();
            this.gbxCheckRules = new System.Windows.Forms.GroupBox();
            this.rbnRCEqual = new System.Windows.Forms.RadioButton();
            this.rbnRCTeamB = new System.Windows.Forms.RadioButton();
            this.rbnRCTeamA = new System.Windows.Forms.RadioButton();
            this.rbnNoRedCard = new System.Windows.Forms.RadioButton();
            this.spnGoalSumHi = new System.Windows.Forms.NumericUpDown();
            this.lblAnd2 = new System.Windows.Forms.Label();
            this.spnGoalSumLo = new System.Windows.Forms.NumericUpDown();
            this.lblBetween2 = new System.Windows.Forms.Label();
            this.chkCheckGoalSum = new System.Windows.Forms.CheckBox();
            this.spnPlaytimeHi = new System.Windows.Forms.NumericUpDown();
            this.lblAnd1 = new System.Windows.Forms.Label();
            this.spnPlaytimeLo = new System.Windows.Forms.NumericUpDown();
            this.lblBetween1 = new System.Windows.Forms.Label();
            this.chkCheckPlaytime = new System.Windows.Forms.CheckBox();
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
            this.pnlButtons.SuspendLayout();
            this.gbxTrigger.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spnCheckOrder)).BeginInit();
            this.gbxCheckRules.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spnGoalSumHi)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnGoalSumLo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnPlaytimeHi)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnPlaytimeLo)).BeginInit();
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
            // pnlButtons
            // 
            this.pnlButtons.Controls.Add(this.btnCancel);
            this.pnlButtons.Controls.Add(this.btnSave);
            this.pnlButtons.Controls.Add(this.btnNew);
            resources.ApplyResources(this.pnlButtons, "pnlButtons");
            this.pnlButtons.Name = "pnlButtons";
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            resources.ApplyResources(this.btnSave, "btnSave");
            this.btnSave.Name = "btnSave";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnNew
            // 
            resources.ApplyResources(this.btnNew, "btnNew");
            this.btnNew.Name = "btnNew";
            this.btnNew.UseVisualStyleBackColor = true;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // gbxTrigger
            // 
            this.gbxTrigger.Controls.Add(this.spnCheckOrder);
            this.gbxTrigger.Controls.Add(this.lblCheckOder);
            this.gbxTrigger.Controls.Add(this.cbxTrigger);
            this.gbxTrigger.Controls.Add(this.lblTrigger);
            resources.ApplyResources(this.gbxTrigger, "gbxTrigger");
            this.gbxTrigger.Name = "gbxTrigger";
            this.gbxTrigger.TabStop = false;
            // 
            // spnCheckOrder
            // 
            resources.ApplyResources(this.spnCheckOrder, "spnCheckOrder");
            this.spnCheckOrder.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.spnCheckOrder.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.spnCheckOrder.Name = "spnCheckOrder";
            this.spnCheckOrder.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lblCheckOder
            // 
            resources.ApplyResources(this.lblCheckOder, "lblCheckOder");
            this.lblCheckOder.Name = "lblCheckOder";
            // 
            // cbxTrigger
            // 
            this.cbxTrigger.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxTrigger.FormattingEnabled = true;
            resources.ApplyResources(this.cbxTrigger, "cbxTrigger");
            this.cbxTrigger.Name = "cbxTrigger";
            // 
            // lblTrigger
            // 
            resources.ApplyResources(this.lblTrigger, "lblTrigger");
            this.lblTrigger.Name = "lblTrigger";
            // 
            // gbxCheckRules
            // 
            this.gbxCheckRules.Controls.Add(this.rbnRCEqual);
            this.gbxCheckRules.Controls.Add(this.rbnRCTeamB);
            this.gbxCheckRules.Controls.Add(this.rbnRCTeamA);
            this.gbxCheckRules.Controls.Add(this.rbnNoRedCard);
            this.gbxCheckRules.Controls.Add(this.spnGoalSumHi);
            this.gbxCheckRules.Controls.Add(this.lblAnd2);
            this.gbxCheckRules.Controls.Add(this.spnGoalSumLo);
            this.gbxCheckRules.Controls.Add(this.lblBetween2);
            this.gbxCheckRules.Controls.Add(this.chkCheckGoalSum);
            this.gbxCheckRules.Controls.Add(this.spnPlaytimeHi);
            this.gbxCheckRules.Controls.Add(this.lblAnd1);
            this.gbxCheckRules.Controls.Add(this.spnPlaytimeLo);
            this.gbxCheckRules.Controls.Add(this.lblBetween1);
            this.gbxCheckRules.Controls.Add(this.chkCheckPlaytime);
            resources.ApplyResources(this.gbxCheckRules, "gbxCheckRules");
            this.gbxCheckRules.Name = "gbxCheckRules";
            this.gbxCheckRules.TabStop = false;
            // 
            // rbnRCEqual
            // 
            resources.ApplyResources(this.rbnRCEqual, "rbnRCEqual");
            this.rbnRCEqual.Name = "rbnRCEqual";
            this.rbnRCEqual.TabStop = true;
            this.rbnRCEqual.UseVisualStyleBackColor = true;
            // 
            // rbnRCTeamB
            // 
            resources.ApplyResources(this.rbnRCTeamB, "rbnRCTeamB");
            this.rbnRCTeamB.Name = "rbnRCTeamB";
            this.rbnRCTeamB.TabStop = true;
            this.rbnRCTeamB.UseVisualStyleBackColor = true;
            // 
            // rbnRCTeamA
            // 
            resources.ApplyResources(this.rbnRCTeamA, "rbnRCTeamA");
            this.rbnRCTeamA.Name = "rbnRCTeamA";
            this.rbnRCTeamA.TabStop = true;
            this.rbnRCTeamA.UseVisualStyleBackColor = true;
            // 
            // rbnNoRedCard
            // 
            resources.ApplyResources(this.rbnNoRedCard, "rbnNoRedCard");
            this.rbnNoRedCard.Name = "rbnNoRedCard";
            this.rbnNoRedCard.TabStop = true;
            this.rbnNoRedCard.UseVisualStyleBackColor = true;
            // 
            // spnGoalSumHi
            // 
            resources.ApplyResources(this.spnGoalSumHi, "spnGoalSumHi");
            this.spnGoalSumHi.Maximum = new decimal(new int[] {
            90,
            0,
            0,
            0});
            this.spnGoalSumHi.Name = "spnGoalSumHi";
            this.spnGoalSumHi.ValueChanged += new System.EventHandler(this.spnGoalSumHi_ValueChanged);
            // 
            // lblAnd2
            // 
            resources.ApplyResources(this.lblAnd2, "lblAnd2");
            this.lblAnd2.Name = "lblAnd2";
            // 
            // spnGoalSumLo
            // 
            resources.ApplyResources(this.spnGoalSumLo, "spnGoalSumLo");
            this.spnGoalSumLo.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.spnGoalSumLo.Name = "spnGoalSumLo";
            this.spnGoalSumLo.ValueChanged += new System.EventHandler(this.spnGoalSumLo_ValueChanged);
            // 
            // lblBetween2
            // 
            resources.ApplyResources(this.lblBetween2, "lblBetween2");
            this.lblBetween2.Name = "lblBetween2";
            // 
            // chkCheckGoalSum
            // 
            resources.ApplyResources(this.chkCheckGoalSum, "chkCheckGoalSum");
            this.chkCheckGoalSum.Name = "chkCheckGoalSum";
            this.chkCheckGoalSum.UseVisualStyleBackColor = true;
            this.chkCheckGoalSum.CheckedChanged += new System.EventHandler(this.chkCheckGoalSum_CheckedChanged);
            // 
            // spnPlaytimeHi
            // 
            resources.ApplyResources(this.spnPlaytimeHi, "spnPlaytimeHi");
            this.spnPlaytimeHi.Maximum = new decimal(new int[] {
            90,
            0,
            0,
            0});
            this.spnPlaytimeHi.Name = "spnPlaytimeHi";
            this.spnPlaytimeHi.ValueChanged += new System.EventHandler(this.spnPlaytimeHi_ValueChanged);
            // 
            // lblAnd1
            // 
            resources.ApplyResources(this.lblAnd1, "lblAnd1");
            this.lblAnd1.Name = "lblAnd1";
            // 
            // spnPlaytimeLo
            // 
            resources.ApplyResources(this.spnPlaytimeLo, "spnPlaytimeLo");
            this.spnPlaytimeLo.Maximum = new decimal(new int[] {
            90,
            0,
            0,
            0});
            this.spnPlaytimeLo.Name = "spnPlaytimeLo";
            this.spnPlaytimeLo.ValueChanged += new System.EventHandler(this.spnPlaytimeLo_ValueChanged);
            // 
            // lblBetween1
            // 
            resources.ApplyResources(this.lblBetween1, "lblBetween1");
            this.lblBetween1.Name = "lblBetween1";
            // 
            // chkCheckPlaytime
            // 
            resources.ApplyResources(this.chkCheckPlaytime, "chkCheckPlaytime");
            this.chkCheckPlaytime.Name = "chkCheckPlaytime";
            this.chkCheckPlaytime.UseVisualStyleBackColor = true;
            this.chkCheckPlaytime.CheckedChanged += new System.EventHandler(this.chkCheckPlaytime_CheckedChanged);
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
            this.gbxSettings.SizeChanged += new System.EventHandler(this.gbxSettings_SizeChanged);
            // 
            // chkNoTrade
            // 
            resources.ApplyResources(this.chkNoTrade, "chkNoTrade");
            this.chkNoTrade.Name = "chkNoTrade";
            this.chkNoTrade.UseVisualStyleBackColor = true;
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
            // ctlTradeOutRuleEditOver
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gbxSettings);
            this.Controls.Add(this.gbxCheckRules);
            this.Controls.Add(this.gbxTrigger);
            this.Controls.Add(this.pnlButtons);
            this.Name = "ctlTradeOutRuleEditOver";
            this.pnlButtons.ResumeLayout(false);
            this.gbxTrigger.ResumeLayout(false);
            this.gbxTrigger.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spnCheckOrder)).EndInit();
            this.gbxCheckRules.ResumeLayout(false);
            this.gbxCheckRules.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spnGoalSumHi)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnGoalSumLo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnPlaytimeHi)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnPlaytimeLo)).EndInit();
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

        private System.Windows.Forms.Panel pnlButtons;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.GroupBox gbxTrigger;
        private System.Windows.Forms.NumericUpDown spnCheckOrder;
        private System.Windows.Forms.Label lblCheckOder;
        private System.Windows.Forms.ComboBox cbxTrigger;
        private System.Windows.Forms.Label lblTrigger;
        private System.Windows.Forms.GroupBox gbxCheckRules;
        private System.Windows.Forms.CheckBox chkCheckPlaytime;
        private System.Windows.Forms.Label lblBetween1;
        private System.Windows.Forms.Label lblAnd1;
        private System.Windows.Forms.NumericUpDown spnPlaytimeLo;
        private System.Windows.Forms.NumericUpDown spnPlaytimeHi;
        private System.Windows.Forms.CheckBox chkCheckGoalSum;
        private System.Windows.Forms.NumericUpDown spnGoalSumHi;
        private System.Windows.Forms.Label lblAnd2;
        private System.Windows.Forms.NumericUpDown spnGoalSumLo;
        private System.Windows.Forms.Label lblBetween2;
        private System.Windows.Forms.RadioButton rbnNoRedCard;
        private System.Windows.Forms.RadioButton rbnRCTeamA;
        private System.Windows.Forms.RadioButton rbnRCTeamB;
        private System.Windows.Forms.RadioButton rbnRCEqual;
        private System.Windows.Forms.GroupBox gbxSettings;
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
        private System.Windows.Forms.GroupBox gbxPlaytime;
        private System.Windows.Forms.NumericUpDown spnGreenPlaytime;
        private System.Windows.Forms.Label lblGreenPlaytime;
        private System.Windows.Forms.NumericUpDown spnHedgePlaytime;
        private System.Windows.Forms.Label lblHedgePlaytime;
        private System.Windows.Forms.CheckBox chkUseHedgeWaittime;
        private System.Windows.Forms.CheckBox chkUseGreenWaittime;
        private System.Windows.Forms.CheckBox chkNoTrade;
    }
}
