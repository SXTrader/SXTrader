namespace net.sxtrader.bftradingstrategies.BackThe4
{
    partial class ctlConfigBT4
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
            this.gbxConfig = new System.Windows.Forms.GroupBox();
            this.tbcConfig = new System.Windows.Forms.TabControl();
            this.tbpStrategy = new System.Windows.Forms.TabPage();
            this.lblMinutesTrade = new System.Windows.Forms.Label();
            this.spnRestartMinutes = new System.Windows.Forms.NumericUpDown();
            this.lblRestartTrade = new System.Windows.Forms.Label();
            this.gbxStopLoss = new System.Windows.Forms.GroupBox();
            this.lblEndBackOdds = new System.Windows.Forms.Label();
            this.spnOdds = new System.Windows.Forms.NumericUpDown();
            this.lblLowerThan = new System.Windows.Forms.Label();
            this.lblTimes = new System.Windows.Forms.Label();
            this.spnOddsTimes = new System.Windows.Forms.NumericUpDown();
            this.lblStopLooseOdds = new System.Windows.Forms.Label();
            this.lblMinutes = new System.Windows.Forms.Label();
            this.spnMinutesStopLoose = new System.Windows.Forms.NumericUpDown();
            this.lblStopLoose = new System.Windows.Forms.Label();
            this.gbxGoalBehavior = new System.Windows.Forms.GroupBox();
            this.lblDontClose = new System.Windows.Forms.Label();
            this.spnProfit = new System.Windows.Forms.NumericUpDown();
            this.lblProfitLower = new System.Windows.Forms.Label();
            this.cbxNoProfit = new System.Windows.Forms.CheckBox();
            this.lblBeforeStart = new System.Windows.Forms.Label();
            this.spnWaitSeconds = new System.Windows.Forms.NumericUpDown();
            this.lblWait = new System.Windows.Forms.Label();
            this.lblGoals = new System.Windows.Forms.Label();
            this.spnGoals = new System.Windows.Forms.NumericUpDown();
            this.lblStartCloseTradeAfter = new System.Windows.Forms.Label();
            this.cbxDefaultActivation = new System.Windows.Forms.CheckBox();
            this.tbpSound = new System.Windows.Forms.TabPage();
            this.gbxTradingChanged = new System.Windows.Forms.GroupBox();
            this.btnTradingChanged = new System.Windows.Forms.Button();
            this.txtTradingChanged = new System.Windows.Forms.TextBox();
            this.cbxPlayTradingChanged = new System.Windows.Forms.CheckBox();
            this.gbxScoreChanged = new System.Windows.Forms.GroupBox();
            this.btnScoreChanged = new System.Windows.Forms.Button();
            this.txtScoreChanged = new System.Windows.Forms.TextBox();
            this.cbxPlayScoreChanged = new System.Windows.Forms.CheckBox();
            this.gbxGameEnded = new System.Windows.Forms.GroupBox();
            this.btnGameEnded = new System.Windows.Forms.Button();
            this.txtGameEnded = new System.Windows.Forms.TextBox();
            this.cbxPlayGameEnded = new System.Windows.Forms.CheckBox();
            this.gbxMatchAdded = new System.Windows.Forms.GroupBox();
            this.btnMatchAdded = new System.Windows.Forms.Button();
            this.txtMatchAdded = new System.Windows.Forms.TextBox();
            this.cbxPlayMatchAdded = new System.Windows.Forms.CheckBox();
            this.cbxPlaySounds = new System.Windows.Forms.CheckBox();
            this.gbxConfig.SuspendLayout();
            this.tbcConfig.SuspendLayout();
            this.tbpStrategy.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spnRestartMinutes)).BeginInit();
            this.gbxStopLoss.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spnOdds)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnOddsTimes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnMinutesStopLoose)).BeginInit();
            this.gbxGoalBehavior.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spnProfit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnWaitSeconds)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnGoals)).BeginInit();
            this.tbpSound.SuspendLayout();
            this.gbxTradingChanged.SuspendLayout();
            this.gbxScoreChanged.SuspendLayout();
            this.gbxGameEnded.SuspendLayout();
            this.gbxMatchAdded.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbxConfig
            // 
            this.gbxConfig.Controls.Add(this.tbcConfig);
            this.gbxConfig.Location = new System.Drawing.Point(4, 0);
            this.gbxConfig.Name = "gbxConfig";
            this.gbxConfig.Size = new System.Drawing.Size(545, 349);
            this.gbxConfig.TabIndex = 0;
            this.gbxConfig.TabStop = false;
            this.gbxConfig.Text = "Configuration Back The 4";
            // 
            // tbcConfig
            // 
            this.tbcConfig.Controls.Add(this.tbpStrategy);
            this.tbcConfig.Controls.Add(this.tbpSound);
            this.tbcConfig.Location = new System.Drawing.Point(8, 16);
            this.tbcConfig.Name = "tbcConfig";
            this.tbcConfig.SelectedIndex = 0;
            this.tbcConfig.Size = new System.Drawing.Size(531, 327);
            this.tbcConfig.TabIndex = 0;
            // 
            // tbpStrategy
            // 
            this.tbpStrategy.Controls.Add(this.lblMinutesTrade);
            this.tbpStrategy.Controls.Add(this.spnRestartMinutes);
            this.tbpStrategy.Controls.Add(this.lblRestartTrade);
            this.tbpStrategy.Controls.Add(this.gbxStopLoss);
            this.tbpStrategy.Controls.Add(this.gbxGoalBehavior);
            this.tbpStrategy.Controls.Add(this.cbxDefaultActivation);
            this.tbpStrategy.Location = new System.Drawing.Point(4, 22);
            this.tbpStrategy.Name = "tbpStrategy";
            this.tbpStrategy.Padding = new System.Windows.Forms.Padding(3);
            this.tbpStrategy.Size = new System.Drawing.Size(523, 301);
            this.tbpStrategy.TabIndex = 0;
            this.tbpStrategy.Text = "Strategy Controlling";
            this.tbpStrategy.UseVisualStyleBackColor = true;
            // 
            // lblMinutesTrade
            // 
            this.lblMinutesTrade.AutoSize = true;
            this.lblMinutesTrade.Location = new System.Drawing.Point(171, 254);
            this.lblMinutesTrade.Name = "lblMinutesTrade";
            this.lblMinutesTrade.Size = new System.Drawing.Size(151, 13);
            this.lblMinutesTrade.TabIndex = 5;
            this.lblMinutesTrade.Text = "minutes if trade isn\'t completed";
            // 
            // spnRestartMinutes
            // 
            this.spnRestartMinutes.Location = new System.Drawing.Point(113, 250);
            this.spnRestartMinutes.Maximum = new decimal(new int[] {
            600,
            0,
            0,
            0});
            this.spnRestartMinutes.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.spnRestartMinutes.Name = "spnRestartMinutes";
            this.spnRestartMinutes.Size = new System.Drawing.Size(52, 20);
            this.spnRestartMinutes.TabIndex = 4;
            this.spnRestartMinutes.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // lblRestartTrade
            // 
            this.lblRestartTrade.AutoSize = true;
            this.lblRestartTrade.Location = new System.Drawing.Point(9, 254);
            this.lblRestartTrade.Name = "lblRestartTrade";
            this.lblRestartTrade.Size = new System.Drawing.Size(99, 13);
            this.lblRestartTrade.TabIndex = 3;
            this.lblRestartTrade.Text = "Restart Trade after ";
            // 
            // gbxStopLoss
            // 
            this.gbxStopLoss.Controls.Add(this.lblEndBackOdds);
            this.gbxStopLoss.Controls.Add(this.spnOdds);
            this.gbxStopLoss.Controls.Add(this.lblLowerThan);
            this.gbxStopLoss.Controls.Add(this.lblTimes);
            this.gbxStopLoss.Controls.Add(this.spnOddsTimes);
            this.gbxStopLoss.Controls.Add(this.lblStopLooseOdds);
            this.gbxStopLoss.Controls.Add(this.lblMinutes);
            this.gbxStopLoss.Controls.Add(this.spnMinutesStopLoose);
            this.gbxStopLoss.Controls.Add(this.lblStopLoose);
            this.gbxStopLoss.Location = new System.Drawing.Point(9, 150);
            this.gbxStopLoss.Name = "gbxStopLoss";
            this.gbxStopLoss.Size = new System.Drawing.Size(508, 83);
            this.gbxStopLoss.TabIndex = 2;
            this.gbxStopLoss.TabStop = false;
            this.gbxStopLoss.Text = "Stop/Loose Behavior";
            // 
            // lblEndBackOdds
            // 
            this.lblEndBackOdds.AutoSize = true;
            this.lblEndBackOdds.Location = new System.Drawing.Point(142, 62);
            this.lblEndBackOdds.Name = "lblEndBackOdds";
            this.lblEndBackOdds.Size = new System.Drawing.Size(141, 13);
            this.lblEndBackOdds.TabIndex = 8;
            this.lblEndBackOdds.Text = "times of average Back-Odds";
            this.lblEndBackOdds.Click += new System.EventHandler(this.label1_Click);
            // 
            // spnOdds
            // 
            this.spnOdds.DecimalPlaces = 2;
            this.spnOdds.Location = new System.Drawing.Point(87, 58);
            this.spnOdds.Name = "spnOdds";
            this.spnOdds.Size = new System.Drawing.Size(49, 20);
            this.spnOdds.TabIndex = 7;
            // 
            // lblLowerThan
            // 
            this.lblLowerThan.AutoSize = true;
            this.lblLowerThan.Location = new System.Drawing.Point(7, 62);
            this.lblLowerThan.Name = "lblLowerThan";
            this.lblLowerThan.Size = new System.Drawing.Size(74, 13);
            this.lblLowerThan.TabIndex = 6;
            this.lblLowerThan.Text = "but lower than";
            // 
            // lblTimes
            // 
            this.lblTimes.AutoSize = true;
            this.lblTimes.Location = new System.Drawing.Point(259, 39);
            this.lblTimes.Name = "lblTimes";
            this.lblTimes.Size = new System.Drawing.Size(159, 13);
            this.lblTimes.TabIndex = 5;
            this.lblTimes.Text = "times of the average Back-Odds";
            // 
            // spnOddsTimes
            // 
            this.spnOddsTimes.DecimalPlaces = 2;
            this.spnOddsTimes.Location = new System.Drawing.Point(199, 35);
            this.spnOddsTimes.Name = "spnOddsTimes";
            this.spnOddsTimes.Size = new System.Drawing.Size(54, 20);
            this.spnOddsTimes.TabIndex = 4;
            // 
            // lblStopLooseOdds
            // 
            this.lblStopLooseOdds.AutoSize = true;
            this.lblStopLooseOdds.Location = new System.Drawing.Point(7, 39);
            this.lblStopLooseOdds.Name = "lblStopLooseOdds";
            this.lblStopLooseOdds.Size = new System.Drawing.Size(192, 13);
            this.lblStopLooseOdds.TabIndex = 3;
            this.lblStopLooseOdds.Text = "Stop/Loose Trade when Lay-Odds are ";
            // 
            // lblMinutes
            // 
            this.lblMinutes.AutoSize = true;
            this.lblMinutes.Location = new System.Drawing.Point(169, 16);
            this.lblMinutes.Name = "lblMinutes";
            this.lblMinutes.Size = new System.Drawing.Size(96, 13);
            this.lblMinutes.TabIndex = 2;
            this.lblMinutes.Text = "minutes of playtime";
            // 
            // spnMinutesStopLoose
            // 
            this.spnMinutesStopLoose.Location = new System.Drawing.Point(124, 12);
            this.spnMinutesStopLoose.Maximum = new decimal(new int[] {
            90,
            0,
            0,
            0});
            this.spnMinutesStopLoose.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.spnMinutesStopLoose.Name = "spnMinutesStopLoose";
            this.spnMinutesStopLoose.Size = new System.Drawing.Size(40, 20);
            this.spnMinutesStopLoose.TabIndex = 1;
            this.spnMinutesStopLoose.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lblStopLoose
            // 
            this.lblStopLoose.AutoSize = true;
            this.lblStopLoose.Location = new System.Drawing.Point(7, 16);
            this.lblStopLoose.Name = "lblStopLoose";
            this.lblStopLoose.Size = new System.Drawing.Size(112, 13);
            this.lblStopLoose.TabIndex = 0;
            this.lblStopLoose.Text = "Start Stop/Loose after";
            // 
            // gbxGoalBehavior
            // 
            this.gbxGoalBehavior.Controls.Add(this.lblDontClose);
            this.gbxGoalBehavior.Controls.Add(this.spnProfit);
            this.gbxGoalBehavior.Controls.Add(this.lblProfitLower);
            this.gbxGoalBehavior.Controls.Add(this.cbxNoProfit);
            this.gbxGoalBehavior.Controls.Add(this.lblBeforeStart);
            this.gbxGoalBehavior.Controls.Add(this.spnWaitSeconds);
            this.gbxGoalBehavior.Controls.Add(this.lblWait);
            this.gbxGoalBehavior.Controls.Add(this.lblGoals);
            this.gbxGoalBehavior.Controls.Add(this.spnGoals);
            this.gbxGoalBehavior.Controls.Add(this.lblStartCloseTradeAfter);
            this.gbxGoalBehavior.Location = new System.Drawing.Point(9, 33);
            this.gbxGoalBehavior.Name = "gbxGoalBehavior";
            this.gbxGoalBehavior.Size = new System.Drawing.Size(508, 108);
            this.gbxGoalBehavior.TabIndex = 1;
            this.gbxGoalBehavior.TabStop = false;
            this.gbxGoalBehavior.Text = "Close-Trade Behavior";
            // 
            // lblDontClose
            // 
            this.lblDontClose.AutoSize = true;
            this.lblDontClose.Location = new System.Drawing.Point(157, 86);
            this.lblDontClose.Name = "lblDontClose";
            this.lblDontClose.Size = new System.Drawing.Size(96, 13);
            this.lblDontClose.TabIndex = 9;
            this.lblDontClose.Text = "% don\'t close trade";
            // 
            // spnProfit
            // 
            this.spnProfit.Location = new System.Drawing.Point(109, 84);
            this.spnProfit.Name = "spnProfit";
            this.spnProfit.Size = new System.Drawing.Size(47, 20);
            this.spnProfit.TabIndex = 8;
            // 
            // lblProfitLower
            // 
            this.lblProfitLower.AutoSize = true;
            this.lblProfitLower.Location = new System.Drawing.Point(7, 86);
            this.lblProfitLower.Name = "lblProfitLower";
            this.lblProfitLower.Size = new System.Drawing.Size(101, 13);
            this.lblProfitLower.TabIndex = 7;
            this.lblProfitLower.Text = "If profit is lower than";
            // 
            // cbxNoProfit
            // 
            this.cbxNoProfit.AutoSize = true;
            this.cbxNoProfit.Location = new System.Drawing.Point(7, 60);
            this.cbxNoProfit.Name = "cbxNoProfit";
            this.cbxNoProfit.Size = new System.Drawing.Size(193, 17);
            this.cbxNoProfit.TabIndex = 6;
            this.cbxNoProfit.Text = "Close Trade even if there\'s no profit";
            this.cbxNoProfit.UseVisualStyleBackColor = true;
            // 
            // lblBeforeStart
            // 
            this.lblBeforeStart.AutoSize = true;
            this.lblBeforeStart.Location = new System.Drawing.Point(101, 39);
            this.lblBeforeStart.Name = "lblBeforeStart";
            this.lblBeforeStart.Size = new System.Drawing.Size(134, 13);
            this.lblBeforeStart.TabIndex = 5;
            this.lblBeforeStart.Text = "before starting Close-Trade";
            // 
            // spnWaitSeconds
            // 
            this.spnWaitSeconds.Location = new System.Drawing.Point(46, 35);
            this.spnWaitSeconds.Maximum = new decimal(new int[] {
            600,
            0,
            0,
            0});
            this.spnWaitSeconds.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.spnWaitSeconds.Name = "spnWaitSeconds";
            this.spnWaitSeconds.Size = new System.Drawing.Size(45, 20);
            this.spnWaitSeconds.TabIndex = 4;
            this.spnWaitSeconds.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // lblWait
            // 
            this.lblWait.AutoSize = true;
            this.lblWait.Location = new System.Drawing.Point(7, 38);
            this.lblWait.Name = "lblWait";
            this.lblWait.Size = new System.Drawing.Size(29, 13);
            this.lblWait.TabIndex = 3;
            this.lblWait.Text = "Wait";
            // 
            // lblGoals
            // 
            this.lblGoals.AutoSize = true;
            this.lblGoals.Location = new System.Drawing.Point(171, 16);
            this.lblGoals.Name = "lblGoals";
            this.lblGoals.Size = new System.Drawing.Size(34, 13);
            this.lblGoals.TabIndex = 2;
            this.lblGoals.Text = "Goals";
            // 
            // spnGoals
            // 
            this.spnGoals.Location = new System.Drawing.Point(126, 12);
            this.spnGoals.Maximum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.spnGoals.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.spnGoals.Name = "spnGoals";
            this.spnGoals.Size = new System.Drawing.Size(39, 20);
            this.spnGoals.TabIndex = 1;
            this.spnGoals.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lblStartCloseTradeAfter
            // 
            this.lblStartCloseTradeAfter.AutoSize = true;
            this.lblStartCloseTradeAfter.Location = new System.Drawing.Point(7, 16);
            this.lblStartCloseTradeAfter.Name = "lblStartCloseTradeAfter";
            this.lblStartCloseTradeAfter.Size = new System.Drawing.Size(113, 13);
            this.lblStartCloseTradeAfter.TabIndex = 0;
            this.lblStartCloseTradeAfter.Text = "Start Close-Trade after";
            // 
            // cbxDefaultActivation
            // 
            this.cbxDefaultActivation.AutoSize = true;
            this.cbxDefaultActivation.Location = new System.Drawing.Point(7, 7);
            this.cbxDefaultActivation.Name = "cbxDefaultActivation";
            this.cbxDefaultActivation.Size = new System.Drawing.Size(177, 17);
            this.cbxDefaultActivation.TabIndex = 0;
            this.cbxDefaultActivation.Text = "Strategy is activated by default?";
            this.cbxDefaultActivation.UseVisualStyleBackColor = true;
            // 
            // tbpSound
            // 
            this.tbpSound.Controls.Add(this.gbxTradingChanged);
            this.tbpSound.Controls.Add(this.gbxScoreChanged);
            this.tbpSound.Controls.Add(this.gbxGameEnded);
            this.tbpSound.Controls.Add(this.gbxMatchAdded);
            this.tbpSound.Controls.Add(this.cbxPlaySounds);
            this.tbpSound.Location = new System.Drawing.Point(4, 22);
            this.tbpSound.Name = "tbpSound";
            this.tbpSound.Padding = new System.Windows.Forms.Padding(3);
            this.tbpSound.Size = new System.Drawing.Size(523, 301);
            this.tbpSound.TabIndex = 2;
            this.tbpSound.Text = "Sound";
            this.tbpSound.UseVisualStyleBackColor = true;
            // 
            // gbxTradingChanged
            // 
            this.gbxTradingChanged.Controls.Add(this.btnTradingChanged);
            this.gbxTradingChanged.Controls.Add(this.txtTradingChanged);
            this.gbxTradingChanged.Controls.Add(this.cbxPlayTradingChanged);
            this.gbxTradingChanged.Dock = System.Windows.Forms.DockStyle.Top;
            this.gbxTradingChanged.Location = new System.Drawing.Point(3, 227);
            this.gbxTradingChanged.Name = "gbxTradingChanged";
            this.gbxTradingChanged.Size = new System.Drawing.Size(517, 69);
            this.gbxTradingChanged.TabIndex = 4;
            this.gbxTradingChanged.TabStop = false;
            this.gbxTradingChanged.Text = "Trading Changed";
            // 
            // btnTradingChanged
            // 
            this.btnTradingChanged.Location = new System.Drawing.Point(346, 40);
            this.btnTradingChanged.Name = "btnTradingChanged";
            this.btnTradingChanged.Size = new System.Drawing.Size(75, 23);
            this.btnTradingChanged.TabIndex = 11;
            this.btnTradingChanged.Text = "&Browse...";
            this.btnTradingChanged.UseVisualStyleBackColor = true;
            this.btnTradingChanged.Click += new System.EventHandler(this.btnTradingChanged_Click);
            // 
            // txtTradingChanged
            // 
            this.txtTradingChanged.Location = new System.Drawing.Point(6, 43);
            this.txtTradingChanged.Name = "txtTradingChanged";
            this.txtTradingChanged.Size = new System.Drawing.Size(321, 20);
            this.txtTradingChanged.TabIndex = 10;
            // 
            // cbxPlayTradingChanged
            // 
            this.cbxPlayTradingChanged.AutoSize = true;
            this.cbxPlayTradingChanged.Location = new System.Drawing.Point(6, 19);
            this.cbxPlayTradingChanged.Name = "cbxPlayTradingChanged";
            this.cbxPlayTradingChanged.Size = new System.Drawing.Size(46, 17);
            this.cbxPlayTradingChanged.TabIndex = 9;
            this.cbxPlayTradingChanged.Text = "Play";
            this.cbxPlayTradingChanged.UseVisualStyleBackColor = true;
            // 
            // gbxScoreChanged
            // 
            this.gbxScoreChanged.Controls.Add(this.btnScoreChanged);
            this.gbxScoreChanged.Controls.Add(this.txtScoreChanged);
            this.gbxScoreChanged.Controls.Add(this.cbxPlayScoreChanged);
            this.gbxScoreChanged.Dock = System.Windows.Forms.DockStyle.Top;
            this.gbxScoreChanged.Location = new System.Drawing.Point(3, 158);
            this.gbxScoreChanged.Name = "gbxScoreChanged";
            this.gbxScoreChanged.Size = new System.Drawing.Size(517, 69);
            this.gbxScoreChanged.TabIndex = 3;
            this.gbxScoreChanged.TabStop = false;
            this.gbxScoreChanged.Text = "Score Changed";
            // 
            // btnScoreChanged
            // 
            this.btnScoreChanged.Location = new System.Drawing.Point(346, 40);
            this.btnScoreChanged.Name = "btnScoreChanged";
            this.btnScoreChanged.Size = new System.Drawing.Size(75, 23);
            this.btnScoreChanged.TabIndex = 8;
            this.btnScoreChanged.Text = "&Browse...";
            this.btnScoreChanged.UseVisualStyleBackColor = true;
            this.btnScoreChanged.Click += new System.EventHandler(this.btnScoreChanged_Click);
            // 
            // txtScoreChanged
            // 
            this.txtScoreChanged.Location = new System.Drawing.Point(6, 43);
            this.txtScoreChanged.Name = "txtScoreChanged";
            this.txtScoreChanged.Size = new System.Drawing.Size(321, 20);
            this.txtScoreChanged.TabIndex = 7;
            // 
            // cbxPlayScoreChanged
            // 
            this.cbxPlayScoreChanged.AutoSize = true;
            this.cbxPlayScoreChanged.Location = new System.Drawing.Point(6, 19);
            this.cbxPlayScoreChanged.Name = "cbxPlayScoreChanged";
            this.cbxPlayScoreChanged.Size = new System.Drawing.Size(46, 17);
            this.cbxPlayScoreChanged.TabIndex = 6;
            this.cbxPlayScoreChanged.Text = "Play";
            this.cbxPlayScoreChanged.UseVisualStyleBackColor = true;
            // 
            // gbxGameEnded
            // 
            this.gbxGameEnded.Controls.Add(this.btnGameEnded);
            this.gbxGameEnded.Controls.Add(this.txtGameEnded);
            this.gbxGameEnded.Controls.Add(this.cbxPlayGameEnded);
            this.gbxGameEnded.Dock = System.Windows.Forms.DockStyle.Top;
            this.gbxGameEnded.Location = new System.Drawing.Point(3, 89);
            this.gbxGameEnded.Name = "gbxGameEnded";
            this.gbxGameEnded.Size = new System.Drawing.Size(517, 69);
            this.gbxGameEnded.TabIndex = 2;
            this.gbxGameEnded.TabStop = false;
            this.gbxGameEnded.Text = "Game Ended";
            // 
            // btnGameEnded
            // 
            this.btnGameEnded.Location = new System.Drawing.Point(346, 40);
            this.btnGameEnded.Name = "btnGameEnded";
            this.btnGameEnded.Size = new System.Drawing.Size(75, 23);
            this.btnGameEnded.TabIndex = 5;
            this.btnGameEnded.Text = "&Browse...";
            this.btnGameEnded.UseVisualStyleBackColor = true;
            this.btnGameEnded.Click += new System.EventHandler(this.btnGameEnded_Click);
            // 
            // txtGameEnded
            // 
            this.txtGameEnded.Location = new System.Drawing.Point(6, 43);
            this.txtGameEnded.Name = "txtGameEnded";
            this.txtGameEnded.Size = new System.Drawing.Size(321, 20);
            this.txtGameEnded.TabIndex = 4;
            // 
            // cbxPlayGameEnded
            // 
            this.cbxPlayGameEnded.AutoSize = true;
            this.cbxPlayGameEnded.Location = new System.Drawing.Point(6, 19);
            this.cbxPlayGameEnded.Name = "cbxPlayGameEnded";
            this.cbxPlayGameEnded.Size = new System.Drawing.Size(46, 17);
            this.cbxPlayGameEnded.TabIndex = 3;
            this.cbxPlayGameEnded.Text = "Play";
            this.cbxPlayGameEnded.UseVisualStyleBackColor = true;
            // 
            // gbxMatchAdded
            // 
            this.gbxMatchAdded.Controls.Add(this.btnMatchAdded);
            this.gbxMatchAdded.Controls.Add(this.txtMatchAdded);
            this.gbxMatchAdded.Controls.Add(this.cbxPlayMatchAdded);
            this.gbxMatchAdded.Dock = System.Windows.Forms.DockStyle.Top;
            this.gbxMatchAdded.Location = new System.Drawing.Point(3, 20);
            this.gbxMatchAdded.Name = "gbxMatchAdded";
            this.gbxMatchAdded.Size = new System.Drawing.Size(517, 69);
            this.gbxMatchAdded.TabIndex = 1;
            this.gbxMatchAdded.TabStop = false;
            this.gbxMatchAdded.Text = "MatchAdded";
            // 
            // btnMatchAdded
            // 
            this.btnMatchAdded.Location = new System.Drawing.Point(346, 40);
            this.btnMatchAdded.Name = "btnMatchAdded";
            this.btnMatchAdded.Size = new System.Drawing.Size(75, 23);
            this.btnMatchAdded.TabIndex = 2;
            this.btnMatchAdded.Text = "&Browse...";
            this.btnMatchAdded.UseVisualStyleBackColor = true;
            this.btnMatchAdded.Click += new System.EventHandler(this.btnMatchAdded_Click);
            // 
            // txtMatchAdded
            // 
            this.txtMatchAdded.Location = new System.Drawing.Point(6, 43);
            this.txtMatchAdded.Name = "txtMatchAdded";
            this.txtMatchAdded.Size = new System.Drawing.Size(321, 20);
            this.txtMatchAdded.TabIndex = 1;
            // 
            // cbxPlayMatchAdded
            // 
            this.cbxPlayMatchAdded.AutoSize = true;
            this.cbxPlayMatchAdded.Location = new System.Drawing.Point(6, 19);
            this.cbxPlayMatchAdded.Name = "cbxPlayMatchAdded";
            this.cbxPlayMatchAdded.Size = new System.Drawing.Size(46, 17);
            this.cbxPlayMatchAdded.TabIndex = 0;
            this.cbxPlayMatchAdded.Text = "Play";
            this.cbxPlayMatchAdded.UseVisualStyleBackColor = true;
            // 
            // cbxPlaySounds
            // 
            this.cbxPlaySounds.AutoSize = true;
            this.cbxPlaySounds.Dock = System.Windows.Forms.DockStyle.Top;
            this.cbxPlaySounds.Location = new System.Drawing.Point(3, 3);
            this.cbxPlaySounds.Name = "cbxPlaySounds";
            this.cbxPlaySounds.Size = new System.Drawing.Size(517, 17);
            this.cbxPlaySounds.TabIndex = 0;
            this.cbxPlaySounds.Text = "Play Sounds";
            this.cbxPlaySounds.UseVisualStyleBackColor = true;
            // 
            // ctlConfigBT4
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gbxConfig);
            this.Name = "ctlConfigBT4";
            this.Size = new System.Drawing.Size(550, 350);
            this.gbxConfig.ResumeLayout(false);
            this.tbcConfig.ResumeLayout(false);
            this.tbpStrategy.ResumeLayout(false);
            this.tbpStrategy.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spnRestartMinutes)).EndInit();
            this.gbxStopLoss.ResumeLayout(false);
            this.gbxStopLoss.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spnOdds)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnOddsTimes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnMinutesStopLoose)).EndInit();
            this.gbxGoalBehavior.ResumeLayout(false);
            this.gbxGoalBehavior.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spnProfit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnWaitSeconds)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnGoals)).EndInit();
            this.tbpSound.ResumeLayout(false);
            this.tbpSound.PerformLayout();
            this.gbxTradingChanged.ResumeLayout(false);
            this.gbxTradingChanged.PerformLayout();
            this.gbxScoreChanged.ResumeLayout(false);
            this.gbxScoreChanged.PerformLayout();
            this.gbxGameEnded.ResumeLayout(false);
            this.gbxGameEnded.PerformLayout();
            this.gbxMatchAdded.ResumeLayout(false);
            this.gbxMatchAdded.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbxConfig;
        private System.Windows.Forms.TabControl tbcConfig;
        private System.Windows.Forms.TabPage tbpStrategy;
        private System.Windows.Forms.CheckBox cbxDefaultActivation;
        private System.Windows.Forms.GroupBox gbxGoalBehavior;
        private System.Windows.Forms.Label lblStartCloseTradeAfter;
        private System.Windows.Forms.Label lblGoals;
        private System.Windows.Forms.NumericUpDown spnGoals;
        private System.Windows.Forms.NumericUpDown spnWaitSeconds;
        private System.Windows.Forms.Label lblWait;
        private System.Windows.Forms.NumericUpDown spnProfit;
        private System.Windows.Forms.Label lblProfitLower;
        private System.Windows.Forms.CheckBox cbxNoProfit;
        private System.Windows.Forms.Label lblBeforeStart;
        private System.Windows.Forms.Label lblDontClose;
        private System.Windows.Forms.GroupBox gbxStopLoss;
        private System.Windows.Forms.NumericUpDown spnMinutesStopLoose;
        private System.Windows.Forms.Label lblStopLoose;
        private System.Windows.Forms.Label lblMinutes;
        private System.Windows.Forms.NumericUpDown spnOdds;
        private System.Windows.Forms.Label lblLowerThan;
        private System.Windows.Forms.Label lblTimes;
        private System.Windows.Forms.NumericUpDown spnOddsTimes;
        private System.Windows.Forms.Label lblStopLooseOdds;
        private System.Windows.Forms.NumericUpDown spnRestartMinutes;
        private System.Windows.Forms.Label lblRestartTrade;
        private System.Windows.Forms.Label lblMinutesTrade;
        private System.Windows.Forms.Label lblEndBackOdds;
        private System.Windows.Forms.TabPage tbpSound;
        private System.Windows.Forms.GroupBox gbxTradingChanged;
        private System.Windows.Forms.Button btnTradingChanged;
        private System.Windows.Forms.TextBox txtTradingChanged;
        private System.Windows.Forms.CheckBox cbxPlayTradingChanged;
        private System.Windows.Forms.GroupBox gbxScoreChanged;
        private System.Windows.Forms.Button btnScoreChanged;
        private System.Windows.Forms.TextBox txtScoreChanged;
        private System.Windows.Forms.CheckBox cbxPlayScoreChanged;
        private System.Windows.Forms.GroupBox gbxGameEnded;
        private System.Windows.Forms.Button btnGameEnded;
        private System.Windows.Forms.TextBox txtGameEnded;
        private System.Windows.Forms.CheckBox cbxPlayGameEnded;
        private System.Windows.Forms.GroupBox gbxMatchAdded;
        private System.Windows.Forms.Button btnMatchAdded;
        private System.Windows.Forms.TextBox txtMatchAdded;
        private System.Windows.Forms.CheckBox cbxPlayMatchAdded;
        private System.Windows.Forms.CheckBox cbxPlaySounds;
    }
}
