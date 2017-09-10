namespace net.sxtrader.bftradingstrategies.tippsters.GUI.Configuration
{
    partial class ctlConfiguration
    {
        /// <summary> 
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ctlConfiguration));
            this.tbcConfiguration = new System.Windows.Forms.TabControl();
            this.tbpCommon = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lnkSubscription = new System.Windows.Forms.LinkLabel();
            this.lblSubscription = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.chkActive = new System.Windows.Forms.CheckBox();
            this.tbpMail = new System.Windows.Forms.TabPage();
            this.gbxIncoming = new System.Windows.Forms.GroupBox();
            this.btnTest = new System.Windows.Forms.Button();
            this.cbxSSL = new System.Windows.Forms.CheckBox();
            this.lblHours = new System.Windows.Forms.Label();
            this.spnHours = new System.Windows.Forms.NumericUpDown();
            this.lblCheck = new System.Windows.Forms.Label();
            this.txtMailAccess = new System.Windows.Forms.TextBox();
            this.lblMailAccess = new System.Windows.Forms.Label();
            this.txtMailUser = new System.Windows.Forms.TextBox();
            this.lblMailUser = new System.Windows.Forms.Label();
            this.spnIMAPPort = new System.Windows.Forms.NumericUpDown();
            this.lblPort = new System.Windows.Forms.Label();
            this.txtMailHost = new System.Windows.Forms.TextBox();
            this.lblMailServer = new System.Windows.Forms.Label();
            this.tbpTrade = new System.Windows.Forms.TabPage();
            this.lblStartTime2 = new System.Windows.Forms.Label();
            this.spnStartMinute = new System.Windows.Forms.NumericUpDown();
            this.lblTradeStartTime = new System.Windows.Forms.Label();
            this.lblTimeToStart = new System.Windows.Forms.Label();
            this.spnLastTime = new System.Windows.Forms.NumericUpDown();
            this.chkLastBet = new System.Windows.Forms.CheckBox();
            this.lblTicks = new System.Windows.Forms.Label();
            this.spnTicks = new System.Windows.Forms.NumericUpDown();
            this.chkDynamicBetting = new System.Windows.Forms.CheckBox();
            this.gbxNonRunner = new System.Windows.Forms.GroupBox();
            this.spn16To20 = new System.Windows.Forms.NumericUpDown();
            this.lbl16To20 = new System.Windows.Forms.Label();
            this.spn11To15 = new System.Windows.Forms.NumericUpDown();
            this.lbl11To15 = new System.Windows.Forms.Label();
            this.lblNonRunners = new System.Windows.Forms.Label();
            this.spn1To5 = new System.Windows.Forms.NumericUpDown();
            this.spn6To10 = new System.Windows.Forms.NumericUpDown();
            this.spnMoreThan20 = new System.Windows.Forms.NumericUpDown();
            this.lblMoreThan20 = new System.Windows.Forms.Label();
            this.lbl6To10 = new System.Windows.Forms.Label();
            this.lbl1To5 = new System.Windows.Forms.Label();
            this.chkNonRunner = new System.Windows.Forms.CheckBox();
            this.chkPlaceForMaxOdds = new System.Windows.Forms.CheckBox();
            this.chkInRunning = new System.Windows.Forms.CheckBox();
            this.spnMaxOdds = new System.Windows.Forms.NumericUpDown();
            this.lblMaximumOdds = new System.Windows.Forms.Label();
            this.tbpBankroll = new System.Windows.Forms.TabPage();
            this.gbxAmountKind = new System.Windows.Forms.GroupBox();
            this.rbnAsRisk = new System.Windows.Forms.RadioButton();
            this.rbnAsBetAmount = new System.Windows.Forms.RadioButton();
            this.lblCurrency = new System.Windows.Forms.Label();
            this.spnTippAmount = new System.Windows.Forms.NumericUpDown();
            this.lblTipAmount = new System.Windows.Forms.Label();
            this.tbcConfiguration.SuspendLayout();
            this.tbpCommon.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tbpMail.SuspendLayout();
            this.gbxIncoming.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spnHours)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnIMAPPort)).BeginInit();
            this.tbpTrade.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spnStartMinute)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnLastTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnTicks)).BeginInit();
            this.gbxNonRunner.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spn16To20)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spn11To15)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spn1To5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spn6To10)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnMoreThan20)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnMaxOdds)).BeginInit();
            this.tbpBankroll.SuspendLayout();
            this.gbxAmountKind.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spnTippAmount)).BeginInit();
            this.SuspendLayout();
            // 
            // tbcConfiguration
            // 
            this.tbcConfiguration.Controls.Add(this.tbpCommon);
            this.tbcConfiguration.Controls.Add(this.tbpMail);
            this.tbcConfiguration.Controls.Add(this.tbpTrade);
            this.tbcConfiguration.Controls.Add(this.tbpBankroll);
            resources.ApplyResources(this.tbcConfiguration, "tbcConfiguration");
            this.tbcConfiguration.Name = "tbcConfiguration";
            this.tbcConfiguration.SelectedIndex = 0;
            // 
            // tbpCommon
            // 
            this.tbpCommon.Controls.Add(this.groupBox1);
            this.tbpCommon.Controls.Add(this.chkActive);
            resources.ApplyResources(this.tbpCommon, "tbpCommon");
            this.tbpCommon.Name = "tbpCommon";
            this.tbpCommon.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lnkSubscription);
            this.groupBox1.Controls.Add(this.lblSubscription);
            this.groupBox1.Controls.Add(this.label1);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // lnkSubscription
            // 
            resources.ApplyResources(this.lnkSubscription, "lnkSubscription");
            this.lnkSubscription.Name = "lnkSubscription";
            this.lnkSubscription.TabStop = true;
            this.lnkSubscription.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkSubscription_LinkClicked);
            // 
            // lblSubscription
            // 
            resources.ApplyResources(this.lblSubscription, "lblSubscription");
            this.lblSubscription.Name = "lblSubscription";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // chkActive
            // 
            resources.ApplyResources(this.chkActive, "chkActive");
            this.chkActive.Name = "chkActive";
            this.chkActive.UseVisualStyleBackColor = true;
            // 
            // tbpMail
            // 
            this.tbpMail.Controls.Add(this.gbxIncoming);
            resources.ApplyResources(this.tbpMail, "tbpMail");
            this.tbpMail.Name = "tbpMail";
            this.tbpMail.UseVisualStyleBackColor = true;
            // 
            // gbxIncoming
            // 
            this.gbxIncoming.Controls.Add(this.btnTest);
            this.gbxIncoming.Controls.Add(this.cbxSSL);
            this.gbxIncoming.Controls.Add(this.lblHours);
            this.gbxIncoming.Controls.Add(this.spnHours);
            this.gbxIncoming.Controls.Add(this.lblCheck);
            this.gbxIncoming.Controls.Add(this.txtMailAccess);
            this.gbxIncoming.Controls.Add(this.lblMailAccess);
            this.gbxIncoming.Controls.Add(this.txtMailUser);
            this.gbxIncoming.Controls.Add(this.lblMailUser);
            this.gbxIncoming.Controls.Add(this.spnIMAPPort);
            this.gbxIncoming.Controls.Add(this.lblPort);
            this.gbxIncoming.Controls.Add(this.txtMailHost);
            this.gbxIncoming.Controls.Add(this.lblMailServer);
            resources.ApplyResources(this.gbxIncoming, "gbxIncoming");
            this.gbxIncoming.Name = "gbxIncoming";
            this.gbxIncoming.TabStop = false;
            // 
            // btnTest
            // 
            resources.ApplyResources(this.btnTest, "btnTest");
            this.btnTest.Name = "btnTest";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // cbxSSL
            // 
            resources.ApplyResources(this.cbxSSL, "cbxSSL");
            this.cbxSSL.Name = "cbxSSL";
            this.cbxSSL.UseVisualStyleBackColor = true;
            // 
            // lblHours
            // 
            resources.ApplyResources(this.lblHours, "lblHours");
            this.lblHours.Name = "lblHours";
            // 
            // spnHours
            // 
            resources.ApplyResources(this.spnHours, "spnHours");
            this.spnHours.Maximum = new decimal(new int[] {
            1440,
            0,
            0,
            0});
            this.spnHours.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.spnHours.Name = "spnHours";
            this.spnHours.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // lblCheck
            // 
            resources.ApplyResources(this.lblCheck, "lblCheck");
            this.lblCheck.Name = "lblCheck";
            // 
            // txtMailAccess
            // 
            resources.ApplyResources(this.txtMailAccess, "txtMailAccess");
            this.txtMailAccess.Name = "txtMailAccess";
            // 
            // lblMailAccess
            // 
            resources.ApplyResources(this.lblMailAccess, "lblMailAccess");
            this.lblMailAccess.Name = "lblMailAccess";
            // 
            // txtMailUser
            // 
            resources.ApplyResources(this.txtMailUser, "txtMailUser");
            this.txtMailUser.Name = "txtMailUser";
            // 
            // lblMailUser
            // 
            resources.ApplyResources(this.lblMailUser, "lblMailUser");
            this.lblMailUser.Name = "lblMailUser";
            // 
            // spnIMAPPort
            // 
            resources.ApplyResources(this.spnIMAPPort, "spnIMAPPort");
            this.spnIMAPPort.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.spnIMAPPort.Name = "spnIMAPPort";
            this.spnIMAPPort.Value = new decimal(new int[] {
            143,
            0,
            0,
            0});
            // 
            // lblPort
            // 
            resources.ApplyResources(this.lblPort, "lblPort");
            this.lblPort.Name = "lblPort";
            // 
            // txtMailHost
            // 
            resources.ApplyResources(this.txtMailHost, "txtMailHost");
            this.txtMailHost.Name = "txtMailHost";
            // 
            // lblMailServer
            // 
            resources.ApplyResources(this.lblMailServer, "lblMailServer");
            this.lblMailServer.Name = "lblMailServer";
            // 
            // tbpTrade
            // 
            this.tbpTrade.Controls.Add(this.lblStartTime2);
            this.tbpTrade.Controls.Add(this.spnStartMinute);
            this.tbpTrade.Controls.Add(this.lblTradeStartTime);
            this.tbpTrade.Controls.Add(this.lblTimeToStart);
            this.tbpTrade.Controls.Add(this.spnLastTime);
            this.tbpTrade.Controls.Add(this.chkLastBet);
            this.tbpTrade.Controls.Add(this.lblTicks);
            this.tbpTrade.Controls.Add(this.spnTicks);
            this.tbpTrade.Controls.Add(this.chkDynamicBetting);
            this.tbpTrade.Controls.Add(this.gbxNonRunner);
            this.tbpTrade.Controls.Add(this.chkNonRunner);
            this.tbpTrade.Controls.Add(this.chkPlaceForMaxOdds);
            this.tbpTrade.Controls.Add(this.chkInRunning);
            this.tbpTrade.Controls.Add(this.spnMaxOdds);
            this.tbpTrade.Controls.Add(this.lblMaximumOdds);
            resources.ApplyResources(this.tbpTrade, "tbpTrade");
            this.tbpTrade.Name = "tbpTrade";
            this.tbpTrade.UseVisualStyleBackColor = true;
            // 
            // lblStartTime2
            // 
            resources.ApplyResources(this.lblStartTime2, "lblStartTime2");
            this.lblStartTime2.Name = "lblStartTime2";
            // 
            // spnStartMinute
            // 
            resources.ApplyResources(this.spnStartMinute, "spnStartMinute");
            this.spnStartMinute.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.spnStartMinute.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.spnStartMinute.Name = "spnStartMinute";
            this.spnStartMinute.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lblTradeStartTime
            // 
            resources.ApplyResources(this.lblTradeStartTime, "lblTradeStartTime");
            this.lblTradeStartTime.Name = "lblTradeStartTime";
            // 
            // lblTimeToStart
            // 
            resources.ApplyResources(this.lblTimeToStart, "lblTimeToStart");
            this.lblTimeToStart.Name = "lblTimeToStart";
            // 
            // spnLastTime
            // 
            resources.ApplyResources(this.spnLastTime, "spnLastTime");
            this.spnLastTime.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.spnLastTime.Name = "spnLastTime";
            this.spnLastTime.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // chkLastBet
            // 
            resources.ApplyResources(this.chkLastBet, "chkLastBet");
            this.chkLastBet.Name = "chkLastBet";
            this.chkLastBet.UseVisualStyleBackColor = true;
            // 
            // lblTicks
            // 
            resources.ApplyResources(this.lblTicks, "lblTicks");
            this.lblTicks.Name = "lblTicks";
            // 
            // spnTicks
            // 
            resources.ApplyResources(this.spnTicks, "spnTicks");
            this.spnTicks.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.spnTicks.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.spnTicks.Name = "spnTicks";
            this.spnTicks.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // chkDynamicBetting
            // 
            resources.ApplyResources(this.chkDynamicBetting, "chkDynamicBetting");
            this.chkDynamicBetting.Name = "chkDynamicBetting";
            this.chkDynamicBetting.UseVisualStyleBackColor = true;
            // 
            // gbxNonRunner
            // 
            this.gbxNonRunner.Controls.Add(this.spn16To20);
            this.gbxNonRunner.Controls.Add(this.lbl16To20);
            this.gbxNonRunner.Controls.Add(this.spn11To15);
            this.gbxNonRunner.Controls.Add(this.lbl11To15);
            this.gbxNonRunner.Controls.Add(this.lblNonRunners);
            this.gbxNonRunner.Controls.Add(this.spn1To5);
            this.gbxNonRunner.Controls.Add(this.spn6To10);
            this.gbxNonRunner.Controls.Add(this.spnMoreThan20);
            this.gbxNonRunner.Controls.Add(this.lblMoreThan20);
            this.gbxNonRunner.Controls.Add(this.lbl6To10);
            this.gbxNonRunner.Controls.Add(this.lbl1To5);
            resources.ApplyResources(this.gbxNonRunner, "gbxNonRunner");
            this.gbxNonRunner.Name = "gbxNonRunner";
            this.gbxNonRunner.TabStop = false;
            // 
            // spn16To20
            // 
            resources.ApplyResources(this.spn16To20, "spn16To20");
            this.spn16To20.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.spn16To20.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.spn16To20.Name = "spn16To20";
            // 
            // lbl16To20
            // 
            resources.ApplyResources(this.lbl16To20, "lbl16To20");
            this.lbl16To20.Name = "lbl16To20";
            // 
            // spn11To15
            // 
            resources.ApplyResources(this.spn11To15, "spn11To15");
            this.spn11To15.Maximum = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.spn11To15.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.spn11To15.Name = "spn11To15";
            // 
            // lbl11To15
            // 
            resources.ApplyResources(this.lbl11To15, "lbl11To15");
            this.lbl11To15.Name = "lbl11To15";
            // 
            // lblNonRunners
            // 
            resources.ApplyResources(this.lblNonRunners, "lblNonRunners");
            this.lblNonRunners.Name = "lblNonRunners";
            // 
            // spn1To5
            // 
            resources.ApplyResources(this.spn1To5, "spn1To5");
            this.spn1To5.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.spn1To5.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.spn1To5.Name = "spn1To5";
            // 
            // spn6To10
            // 
            resources.ApplyResources(this.spn6To10, "spn6To10");
            this.spn6To10.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.spn6To10.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.spn6To10.Name = "spn6To10";
            // 
            // spnMoreThan20
            // 
            resources.ApplyResources(this.spnMoreThan20, "spnMoreThan20");
            this.spnMoreThan20.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.spnMoreThan20.Name = "spnMoreThan20";
            // 
            // lblMoreThan20
            // 
            resources.ApplyResources(this.lblMoreThan20, "lblMoreThan20");
            this.lblMoreThan20.Name = "lblMoreThan20";
            // 
            // lbl6To10
            // 
            resources.ApplyResources(this.lbl6To10, "lbl6To10");
            this.lbl6To10.Name = "lbl6To10";
            // 
            // lbl1To5
            // 
            resources.ApplyResources(this.lbl1To5, "lbl1To5");
            this.lbl1To5.Name = "lbl1To5";
            // 
            // chkNonRunner
            // 
            resources.ApplyResources(this.chkNonRunner, "chkNonRunner");
            this.chkNonRunner.Name = "chkNonRunner";
            this.chkNonRunner.UseVisualStyleBackColor = true;
            // 
            // chkPlaceForMaxOdds
            // 
            resources.ApplyResources(this.chkPlaceForMaxOdds, "chkPlaceForMaxOdds");
            this.chkPlaceForMaxOdds.Name = "chkPlaceForMaxOdds";
            this.chkPlaceForMaxOdds.UseVisualStyleBackColor = true;
            // 
            // chkInRunning
            // 
            resources.ApplyResources(this.chkInRunning, "chkInRunning");
            this.chkInRunning.Name = "chkInRunning";
            this.chkInRunning.UseVisualStyleBackColor = true;
            this.chkInRunning.CheckedChanged += new System.EventHandler(this.chkInRunning_CheckedChanged);
            // 
            // spnMaxOdds
            // 
            this.spnMaxOdds.DecimalPlaces = 2;
            resources.ApplyResources(this.spnMaxOdds, "spnMaxOdds");
            this.spnMaxOdds.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.spnMaxOdds.Name = "spnMaxOdds";
            // 
            // lblMaximumOdds
            // 
            resources.ApplyResources(this.lblMaximumOdds, "lblMaximumOdds");
            this.lblMaximumOdds.Name = "lblMaximumOdds";
            // 
            // tbpBankroll
            // 
            this.tbpBankroll.Controls.Add(this.gbxAmountKind);
            this.tbpBankroll.Controls.Add(this.lblCurrency);
            this.tbpBankroll.Controls.Add(this.spnTippAmount);
            this.tbpBankroll.Controls.Add(this.lblTipAmount);
            resources.ApplyResources(this.tbpBankroll, "tbpBankroll");
            this.tbpBankroll.Name = "tbpBankroll";
            this.tbpBankroll.UseVisualStyleBackColor = true;
            // 
            // gbxAmountKind
            // 
            this.gbxAmountKind.Controls.Add(this.rbnAsRisk);
            this.gbxAmountKind.Controls.Add(this.rbnAsBetAmount);
            resources.ApplyResources(this.gbxAmountKind, "gbxAmountKind");
            this.gbxAmountKind.Name = "gbxAmountKind";
            this.gbxAmountKind.TabStop = false;
            // 
            // rbnAsRisk
            // 
            resources.ApplyResources(this.rbnAsRisk, "rbnAsRisk");
            this.rbnAsRisk.Name = "rbnAsRisk";
            this.rbnAsRisk.TabStop = true;
            this.rbnAsRisk.UseVisualStyleBackColor = true;
            // 
            // rbnAsBetAmount
            // 
            resources.ApplyResources(this.rbnAsBetAmount, "rbnAsBetAmount");
            this.rbnAsBetAmount.Name = "rbnAsBetAmount";
            this.rbnAsBetAmount.TabStop = true;
            this.rbnAsBetAmount.UseVisualStyleBackColor = true;
            // 
            // lblCurrency
            // 
            resources.ApplyResources(this.lblCurrency, "lblCurrency");
            this.lblCurrency.Name = "lblCurrency";
            // 
            // spnTippAmount
            // 
            this.spnTippAmount.DecimalPlaces = 2;
            resources.ApplyResources(this.spnTippAmount, "spnTippAmount");
            this.spnTippAmount.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.spnTippAmount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.spnTippAmount.Name = "spnTippAmount";
            this.spnTippAmount.Value = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            // 
            // lblTipAmount
            // 
            resources.ApplyResources(this.lblTipAmount, "lblTipAmount");
            this.lblTipAmount.Name = "lblTipAmount";
            // 
            // ctlConfiguration
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tbcConfiguration);
            this.Name = "ctlConfiguration";
            this.tbcConfiguration.ResumeLayout(false);
            this.tbpCommon.ResumeLayout(false);
            this.tbpCommon.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tbpMail.ResumeLayout(false);
            this.gbxIncoming.ResumeLayout(false);
            this.gbxIncoming.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spnHours)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnIMAPPort)).EndInit();
            this.tbpTrade.ResumeLayout(false);
            this.tbpTrade.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spnStartMinute)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnLastTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnTicks)).EndInit();
            this.gbxNonRunner.ResumeLayout(false);
            this.gbxNonRunner.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spn16To20)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spn11To15)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spn1To5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spn6To10)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnMoreThan20)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnMaxOdds)).EndInit();
            this.tbpBankroll.ResumeLayout(false);
            this.tbpBankroll.PerformLayout();
            this.gbxAmountKind.ResumeLayout(false);
            this.gbxAmountKind.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spnTippAmount)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tbcConfiguration;
        private System.Windows.Forms.TabPage tbpMail;
        private System.Windows.Forms.TabPage tbpTrade;
        private System.Windows.Forms.TabPage tbpBankroll;
        private System.Windows.Forms.GroupBox gbxIncoming;
        private System.Windows.Forms.NumericUpDown spnIMAPPort;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.TextBox txtMailHost;
        private System.Windows.Forms.Label lblMailServer;
        private System.Windows.Forms.TextBox txtMailAccess;
        private System.Windows.Forms.Label lblMailAccess;
        private System.Windows.Forms.TextBox txtMailUser;
        private System.Windows.Forms.Label lblMailUser;
        private System.Windows.Forms.Label lblCheck;
        private System.Windows.Forms.Label lblHours;
        private System.Windows.Forms.NumericUpDown spnHours;
        private System.Windows.Forms.NumericUpDown spnMaxOdds;
        private System.Windows.Forms.Label lblMaximumOdds;
        private System.Windows.Forms.CheckBox chkInRunning;
        private System.Windows.Forms.CheckBox chkNonRunner;
        private System.Windows.Forms.GroupBox gbxNonRunner;
        private System.Windows.Forms.Label lblMoreThan20;
        private System.Windows.Forms.Label lbl6To10;
        private System.Windows.Forms.Label lbl1To5;
        private System.Windows.Forms.NumericUpDown spn6To10;
        private System.Windows.Forms.NumericUpDown spnMoreThan20;
        private System.Windows.Forms.NumericUpDown spn1To5;
        private System.Windows.Forms.Label lblTicks;
        private System.Windows.Forms.NumericUpDown spnTicks;
        private System.Windows.Forms.CheckBox chkDynamicBetting;
        private System.Windows.Forms.Label lblNonRunners;
        private System.Windows.Forms.NumericUpDown spn11To15;
        private System.Windows.Forms.Label lbl11To15;
        private System.Windows.Forms.NumericUpDown spn16To20;
        private System.Windows.Forms.Label lbl16To20;
        private System.Windows.Forms.CheckBox chkLastBet;
        private System.Windows.Forms.Label lblTimeToStart;
        private System.Windows.Forms.NumericUpDown spnLastTime;
        private System.Windows.Forms.CheckBox chkPlaceForMaxOdds;
        private System.Windows.Forms.Label lblStartTime2;
        private System.Windows.Forms.NumericUpDown spnStartMinute;
        private System.Windows.Forms.Label lblTradeStartTime;
        private System.Windows.Forms.Label lblTipAmount;
        private System.Windows.Forms.GroupBox gbxAmountKind;
        private System.Windows.Forms.Label lblCurrency;
        private System.Windows.Forms.NumericUpDown spnTippAmount;
        private System.Windows.Forms.RadioButton rbnAsRisk;
        private System.Windows.Forms.RadioButton rbnAsBetAmount;
        private System.Windows.Forms.CheckBox cbxSSL;
        private System.Windows.Forms.TabPage tbpCommon;
        private System.Windows.Forms.CheckBox chkActive;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.LinkLabel lnkSubscription;
        private System.Windows.Forms.Label lblSubscription;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnTest;
    }
}
