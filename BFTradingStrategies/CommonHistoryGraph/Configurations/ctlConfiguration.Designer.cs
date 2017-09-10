namespace net.sxtrader.bftradingstrategies.common
{
    partial class ctlConfiguration
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ctlConfiguration));
            this.tbcConfig = new System.Windows.Forms.TabControl();
            this.tbpGeneral = new System.Windows.Forms.TabPage();
            this.pnlTop = new System.Windows.Forms.Panel();
            this.spnAgeOfData = new System.Windows.Forms.NumericUpDown();
            this.lblAgeOfData = new System.Windows.Forms.Label();
            this.spnHistoricData = new System.Windows.Forms.NumericUpDown();
            this.lblNoHistoricData = new System.Windows.Forms.Label();
            this.tbpScoreMatrix = new System.Windows.Forms.TabPage();
            this.pnlColorRanges = new System.Windows.Forms.Panel();
            this.pnlNewColorRange = new System.Windows.Forms.Panel();
            this.btnNewRange = new System.Windows.Forms.Button();
            this.tbpWLDTrend = new System.Windows.Forms.TabPage();
            this.gbxWLDDrawRangeColors = new System.Windows.Forms.GroupBox();
            this.pnlWLDDrawRanges = new System.Windows.Forms.Panel();
            this.pnlWLDDrawRangeButton = new System.Windows.Forms.Panel();
            this.btnWLDDrawRangeNew = new System.Windows.Forms.Button();
            this.gbxWLDLossRangeColors = new System.Windows.Forms.GroupBox();
            this.pnlWLDLossRanges = new System.Windows.Forms.Panel();
            this.pnlWLDLossRangeButton = new System.Windows.Forms.Panel();
            this.btnWLDLossRangeNew = new System.Windows.Forms.Button();
            this.gbxWLDWinRangeColors = new System.Windows.Forms.GroupBox();
            this.pnlWLDWinRanges = new System.Windows.Forms.Panel();
            this.pnlWLDWinRangeButton = new System.Windows.Forms.Panel();
            this.btnWLDWinRangeNew = new System.Windows.Forms.Button();
            this.gbxWLDTrendColors = new System.Windows.Forms.GroupBox();
            this.cbtWLDTrendZero = new net.sxtrader.muk.ColorButton();
            this.lblWLDTrendZero = new System.Windows.Forms.Label();
            this.cbtWLDTrendDraw = new net.sxtrader.muk.ColorButton();
            this.lblWLDTrendDraw = new System.Windows.Forms.Label();
            this.cbtWLDTrendLoss = new net.sxtrader.muk.ColorButton();
            this.lblWLDTrendLoss = new System.Windows.Forms.Label();
            this.cbtWLDTrendWin = new net.sxtrader.muk.ColorButton();
            this.lblWLDTrendWin = new System.Windows.Forms.Label();
            this.tbpOverUnder = new System.Windows.Forms.TabPage();
            this.gbxOUUnderColorRanges = new System.Windows.Forms.GroupBox();
            this.pnlOUUnderRanges = new System.Windows.Forms.Panel();
            this.pnlOUUnderRangeButton = new System.Windows.Forms.Panel();
            this.btnOUUnderRangeNew = new System.Windows.Forms.Button();
            this.gbxOUOverColorRanges = new System.Windows.Forms.GroupBox();
            this.pnlOUOverRanges = new System.Windows.Forms.Panel();
            this.pnlOUOverRangeButton = new System.Windows.Forms.Panel();
            this.btnOUOverRangeNew = new System.Windows.Forms.Button();
            this.gbxOUTrend = new System.Windows.Forms.GroupBox();
            this.cbtOUTrendUnder = new net.sxtrader.muk.ColorButton();
            this.lblUnder = new System.Windows.Forms.Label();
            this.cbtOUTrendOver = new net.sxtrader.muk.ColorButton();
            this.lblOver = new System.Windows.Forms.Label();
            this.tbpGameColor = new System.Windows.Forms.TabPage();
            this.pnlStatColors = new System.Windows.Forms.Panel();
            this.pnlButtons = new System.Windows.Forms.Panel();
            this.btnNewColorStat = new System.Windows.Forms.Button();
            this.tbcConfig.SuspendLayout();
            this.tbpGeneral.SuspendLayout();
            this.pnlTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spnAgeOfData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnHistoricData)).BeginInit();
            this.tbpScoreMatrix.SuspendLayout();
            this.pnlNewColorRange.SuspendLayout();
            this.tbpWLDTrend.SuspendLayout();
            this.gbxWLDDrawRangeColors.SuspendLayout();
            this.pnlWLDDrawRangeButton.SuspendLayout();
            this.gbxWLDLossRangeColors.SuspendLayout();
            this.pnlWLDLossRangeButton.SuspendLayout();
            this.gbxWLDWinRangeColors.SuspendLayout();
            this.pnlWLDWinRangeButton.SuspendLayout();
            this.gbxWLDTrendColors.SuspendLayout();
            this.tbpOverUnder.SuspendLayout();
            this.gbxOUUnderColorRanges.SuspendLayout();
            this.pnlOUUnderRangeButton.SuspendLayout();
            this.gbxOUOverColorRanges.SuspendLayout();
            this.pnlOUOverRangeButton.SuspendLayout();
            this.gbxOUTrend.SuspendLayout();
            this.tbpGameColor.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbcConfig
            // 
            this.tbcConfig.AccessibleDescription = null;
            this.tbcConfig.AccessibleName = null;
            resources.ApplyResources(this.tbcConfig, "tbcConfig");
            this.tbcConfig.BackgroundImage = null;
            this.tbcConfig.Controls.Add(this.tbpGeneral);
            this.tbcConfig.Controls.Add(this.tbpScoreMatrix);
            this.tbcConfig.Controls.Add(this.tbpWLDTrend);
            this.tbcConfig.Controls.Add(this.tbpOverUnder);
            this.tbcConfig.Controls.Add(this.tbpGameColor);
            this.tbcConfig.Font = null;
            this.tbcConfig.Name = "tbcConfig";
            this.tbcConfig.SelectedIndex = 0;
            // 
            // tbpGeneral
            // 
            this.tbpGeneral.AccessibleDescription = null;
            this.tbpGeneral.AccessibleName = null;
            resources.ApplyResources(this.tbpGeneral, "tbpGeneral");
            this.tbpGeneral.BackgroundImage = null;
            this.tbpGeneral.Controls.Add(this.pnlTop);
            this.tbpGeneral.Font = null;
            this.tbpGeneral.Name = "tbpGeneral";
            this.tbpGeneral.UseVisualStyleBackColor = true;
            // 
            // pnlTop
            // 
            this.pnlTop.AccessibleDescription = null;
            this.pnlTop.AccessibleName = null;
            resources.ApplyResources(this.pnlTop, "pnlTop");
            this.pnlTop.BackgroundImage = null;
            this.pnlTop.Controls.Add(this.spnAgeOfData);
            this.pnlTop.Controls.Add(this.lblAgeOfData);
            this.pnlTop.Controls.Add(this.spnHistoricData);
            this.pnlTop.Controls.Add(this.lblNoHistoricData);
            this.pnlTop.Font = null;
            this.pnlTop.Name = "pnlTop";
            // 
            // spnAgeOfData
            // 
            this.spnAgeOfData.AccessibleDescription = null;
            this.spnAgeOfData.AccessibleName = null;
            resources.ApplyResources(this.spnAgeOfData, "spnAgeOfData");
            this.spnAgeOfData.Font = null;
            this.spnAgeOfData.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.spnAgeOfData.Name = "spnAgeOfData";
            // 
            // lblAgeOfData
            // 
            this.lblAgeOfData.AccessibleDescription = null;
            this.lblAgeOfData.AccessibleName = null;
            resources.ApplyResources(this.lblAgeOfData, "lblAgeOfData");
            this.lblAgeOfData.Font = null;
            this.lblAgeOfData.Name = "lblAgeOfData";
            // 
            // spnHistoricData
            // 
            this.spnHistoricData.AccessibleDescription = null;
            this.spnHistoricData.AccessibleName = null;
            resources.ApplyResources(this.spnHistoricData, "spnHistoricData");
            this.spnHistoricData.Font = null;
            this.spnHistoricData.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.spnHistoricData.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.spnHistoricData.Name = "spnHistoricData";
            this.spnHistoricData.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lblNoHistoricData
            // 
            this.lblNoHistoricData.AccessibleDescription = null;
            this.lblNoHistoricData.AccessibleName = null;
            resources.ApplyResources(this.lblNoHistoricData, "lblNoHistoricData");
            this.lblNoHistoricData.Font = null;
            this.lblNoHistoricData.Name = "lblNoHistoricData";
            // 
            // tbpScoreMatrix
            // 
            this.tbpScoreMatrix.AccessibleDescription = null;
            this.tbpScoreMatrix.AccessibleName = null;
            resources.ApplyResources(this.tbpScoreMatrix, "tbpScoreMatrix");
            this.tbpScoreMatrix.BackgroundImage = null;
            this.tbpScoreMatrix.Controls.Add(this.pnlColorRanges);
            this.tbpScoreMatrix.Controls.Add(this.pnlNewColorRange);
            this.tbpScoreMatrix.Font = null;
            this.tbpScoreMatrix.Name = "tbpScoreMatrix";
            this.tbpScoreMatrix.UseVisualStyleBackColor = true;
            // 
            // pnlColorRanges
            // 
            this.pnlColorRanges.AccessibleDescription = null;
            this.pnlColorRanges.AccessibleName = null;
            resources.ApplyResources(this.pnlColorRanges, "pnlColorRanges");
            this.pnlColorRanges.BackgroundImage = null;
            this.pnlColorRanges.Font = null;
            this.pnlColorRanges.Name = "pnlColorRanges";
            // 
            // pnlNewColorRange
            // 
            this.pnlNewColorRange.AccessibleDescription = null;
            this.pnlNewColorRange.AccessibleName = null;
            resources.ApplyResources(this.pnlNewColorRange, "pnlNewColorRange");
            this.pnlNewColorRange.BackgroundImage = null;
            this.pnlNewColorRange.Controls.Add(this.btnNewRange);
            this.pnlNewColorRange.Font = null;
            this.pnlNewColorRange.Name = "pnlNewColorRange";
            // 
            // btnNewRange
            // 
            this.btnNewRange.AccessibleDescription = null;
            this.btnNewRange.AccessibleName = null;
            resources.ApplyResources(this.btnNewRange, "btnNewRange");
            this.btnNewRange.BackgroundImage = null;
            this.btnNewRange.Font = null;
            this.btnNewRange.Name = "btnNewRange";
            this.btnNewRange.UseVisualStyleBackColor = true;
            this.btnNewRange.Click += new System.EventHandler(this.btnNewRange_Click);
            // 
            // tbpWLDTrend
            // 
            this.tbpWLDTrend.AccessibleDescription = null;
            this.tbpWLDTrend.AccessibleName = null;
            resources.ApplyResources(this.tbpWLDTrend, "tbpWLDTrend");
            this.tbpWLDTrend.BackgroundImage = null;
            this.tbpWLDTrend.Controls.Add(this.gbxWLDDrawRangeColors);
            this.tbpWLDTrend.Controls.Add(this.gbxWLDLossRangeColors);
            this.tbpWLDTrend.Controls.Add(this.gbxWLDWinRangeColors);
            this.tbpWLDTrend.Controls.Add(this.gbxWLDTrendColors);
            this.tbpWLDTrend.Font = null;
            this.tbpWLDTrend.Name = "tbpWLDTrend";
            this.tbpWLDTrend.UseVisualStyleBackColor = true;
            // 
            // gbxWLDDrawRangeColors
            // 
            this.gbxWLDDrawRangeColors.AccessibleDescription = null;
            this.gbxWLDDrawRangeColors.AccessibleName = null;
            resources.ApplyResources(this.gbxWLDDrawRangeColors, "gbxWLDDrawRangeColors");
            this.gbxWLDDrawRangeColors.BackgroundImage = null;
            this.gbxWLDDrawRangeColors.Controls.Add(this.pnlWLDDrawRanges);
            this.gbxWLDDrawRangeColors.Controls.Add(this.pnlWLDDrawRangeButton);
            this.gbxWLDDrawRangeColors.Font = null;
            this.gbxWLDDrawRangeColors.Name = "gbxWLDDrawRangeColors";
            this.gbxWLDDrawRangeColors.TabStop = false;
            // 
            // pnlWLDDrawRanges
            // 
            this.pnlWLDDrawRanges.AccessibleDescription = null;
            this.pnlWLDDrawRanges.AccessibleName = null;
            resources.ApplyResources(this.pnlWLDDrawRanges, "pnlWLDDrawRanges");
            this.pnlWLDDrawRanges.BackgroundImage = null;
            this.pnlWLDDrawRanges.Font = null;
            this.pnlWLDDrawRanges.Name = "pnlWLDDrawRanges";
            // 
            // pnlWLDDrawRangeButton
            // 
            this.pnlWLDDrawRangeButton.AccessibleDescription = null;
            this.pnlWLDDrawRangeButton.AccessibleName = null;
            resources.ApplyResources(this.pnlWLDDrawRangeButton, "pnlWLDDrawRangeButton");
            this.pnlWLDDrawRangeButton.BackgroundImage = null;
            this.pnlWLDDrawRangeButton.Controls.Add(this.btnWLDDrawRangeNew);
            this.pnlWLDDrawRangeButton.Font = null;
            this.pnlWLDDrawRangeButton.Name = "pnlWLDDrawRangeButton";
            // 
            // btnWLDDrawRangeNew
            // 
            this.btnWLDDrawRangeNew.AccessibleDescription = null;
            this.btnWLDDrawRangeNew.AccessibleName = null;
            resources.ApplyResources(this.btnWLDDrawRangeNew, "btnWLDDrawRangeNew");
            this.btnWLDDrawRangeNew.BackgroundImage = null;
            this.btnWLDDrawRangeNew.Font = null;
            this.btnWLDDrawRangeNew.Name = "btnWLDDrawRangeNew";
            this.btnWLDDrawRangeNew.UseVisualStyleBackColor = true;
            this.btnWLDDrawRangeNew.Click += new System.EventHandler(this.btnWLDDrawRangeNew_Click);
            // 
            // gbxWLDLossRangeColors
            // 
            this.gbxWLDLossRangeColors.AccessibleDescription = null;
            this.gbxWLDLossRangeColors.AccessibleName = null;
            resources.ApplyResources(this.gbxWLDLossRangeColors, "gbxWLDLossRangeColors");
            this.gbxWLDLossRangeColors.BackgroundImage = null;
            this.gbxWLDLossRangeColors.Controls.Add(this.pnlWLDLossRanges);
            this.gbxWLDLossRangeColors.Controls.Add(this.pnlWLDLossRangeButton);
            this.gbxWLDLossRangeColors.Font = null;
            this.gbxWLDLossRangeColors.Name = "gbxWLDLossRangeColors";
            this.gbxWLDLossRangeColors.TabStop = false;
            // 
            // pnlWLDLossRanges
            // 
            this.pnlWLDLossRanges.AccessibleDescription = null;
            this.pnlWLDLossRanges.AccessibleName = null;
            resources.ApplyResources(this.pnlWLDLossRanges, "pnlWLDLossRanges");
            this.pnlWLDLossRanges.BackgroundImage = null;
            this.pnlWLDLossRanges.Font = null;
            this.pnlWLDLossRanges.Name = "pnlWLDLossRanges";
            // 
            // pnlWLDLossRangeButton
            // 
            this.pnlWLDLossRangeButton.AccessibleDescription = null;
            this.pnlWLDLossRangeButton.AccessibleName = null;
            resources.ApplyResources(this.pnlWLDLossRangeButton, "pnlWLDLossRangeButton");
            this.pnlWLDLossRangeButton.BackgroundImage = null;
            this.pnlWLDLossRangeButton.Controls.Add(this.btnWLDLossRangeNew);
            this.pnlWLDLossRangeButton.Font = null;
            this.pnlWLDLossRangeButton.Name = "pnlWLDLossRangeButton";
            // 
            // btnWLDLossRangeNew
            // 
            this.btnWLDLossRangeNew.AccessibleDescription = null;
            this.btnWLDLossRangeNew.AccessibleName = null;
            resources.ApplyResources(this.btnWLDLossRangeNew, "btnWLDLossRangeNew");
            this.btnWLDLossRangeNew.BackgroundImage = null;
            this.btnWLDLossRangeNew.Font = null;
            this.btnWLDLossRangeNew.Name = "btnWLDLossRangeNew";
            this.btnWLDLossRangeNew.UseVisualStyleBackColor = true;
            this.btnWLDLossRangeNew.Click += new System.EventHandler(this.btnWLDLossRangeNew_Click);
            // 
            // gbxWLDWinRangeColors
            // 
            this.gbxWLDWinRangeColors.AccessibleDescription = null;
            this.gbxWLDWinRangeColors.AccessibleName = null;
            resources.ApplyResources(this.gbxWLDWinRangeColors, "gbxWLDWinRangeColors");
            this.gbxWLDWinRangeColors.BackgroundImage = null;
            this.gbxWLDWinRangeColors.Controls.Add(this.pnlWLDWinRanges);
            this.gbxWLDWinRangeColors.Controls.Add(this.pnlWLDWinRangeButton);
            this.gbxWLDWinRangeColors.Font = null;
            this.gbxWLDWinRangeColors.Name = "gbxWLDWinRangeColors";
            this.gbxWLDWinRangeColors.TabStop = false;
            // 
            // pnlWLDWinRanges
            // 
            this.pnlWLDWinRanges.AccessibleDescription = null;
            this.pnlWLDWinRanges.AccessibleName = null;
            resources.ApplyResources(this.pnlWLDWinRanges, "pnlWLDWinRanges");
            this.pnlWLDWinRanges.BackgroundImage = null;
            this.pnlWLDWinRanges.Font = null;
            this.pnlWLDWinRanges.Name = "pnlWLDWinRanges";
            // 
            // pnlWLDWinRangeButton
            // 
            this.pnlWLDWinRangeButton.AccessibleDescription = null;
            this.pnlWLDWinRangeButton.AccessibleName = null;
            resources.ApplyResources(this.pnlWLDWinRangeButton, "pnlWLDWinRangeButton");
            this.pnlWLDWinRangeButton.BackgroundImage = null;
            this.pnlWLDWinRangeButton.Controls.Add(this.btnWLDWinRangeNew);
            this.pnlWLDWinRangeButton.Font = null;
            this.pnlWLDWinRangeButton.Name = "pnlWLDWinRangeButton";
            // 
            // btnWLDWinRangeNew
            // 
            this.btnWLDWinRangeNew.AccessibleDescription = null;
            this.btnWLDWinRangeNew.AccessibleName = null;
            resources.ApplyResources(this.btnWLDWinRangeNew, "btnWLDWinRangeNew");
            this.btnWLDWinRangeNew.BackgroundImage = null;
            this.btnWLDWinRangeNew.Font = null;
            this.btnWLDWinRangeNew.Name = "btnWLDWinRangeNew";
            this.btnWLDWinRangeNew.UseVisualStyleBackColor = true;
            this.btnWLDWinRangeNew.Click += new System.EventHandler(this.btnWLDWinRangeNew_Click);
            // 
            // gbxWLDTrendColors
            // 
            this.gbxWLDTrendColors.AccessibleDescription = null;
            this.gbxWLDTrendColors.AccessibleName = null;
            resources.ApplyResources(this.gbxWLDTrendColors, "gbxWLDTrendColors");
            this.gbxWLDTrendColors.BackgroundImage = null;
            this.gbxWLDTrendColors.Controls.Add(this.cbtWLDTrendZero);
            this.gbxWLDTrendColors.Controls.Add(this.lblWLDTrendZero);
            this.gbxWLDTrendColors.Controls.Add(this.cbtWLDTrendDraw);
            this.gbxWLDTrendColors.Controls.Add(this.lblWLDTrendDraw);
            this.gbxWLDTrendColors.Controls.Add(this.cbtWLDTrendLoss);
            this.gbxWLDTrendColors.Controls.Add(this.lblWLDTrendLoss);
            this.gbxWLDTrendColors.Controls.Add(this.cbtWLDTrendWin);
            this.gbxWLDTrendColors.Controls.Add(this.lblWLDTrendWin);
            this.gbxWLDTrendColors.Font = null;
            this.gbxWLDTrendColors.Name = "gbxWLDTrendColors";
            this.gbxWLDTrendColors.TabStop = false;
            // 
            // cbtWLDTrendZero
            // 
            this.cbtWLDTrendZero.AccessibleDescription = null;
            this.cbtWLDTrendZero.AccessibleName = null;
            resources.ApplyResources(this.cbtWLDTrendZero, "cbtWLDTrendZero");
            this.cbtWLDTrendZero.BackgroundImage = null;
            //this.cbtWLDTrendZero.ButtonStyle = 
            this.cbtWLDTrendZero.Font = null;
            this.cbtWLDTrendZero.Name = "cbtWLDTrendZero";
            //this.cbtWLDTrendZero.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.ProfessionalSystem;
            //this.cbtWLDTrendZero.SchemeThemes = ComponentFactory.Krypton.Toolkit.ColorScheme.OfficeStandard;
            //this.cbtWLDTrendZero.Strings.MoreColors = resources.GetString("cbtWLDTrendZero.Strings.MoreColors");
            //this.cbtWLDTrendZero.Strings.NoColor = resources.GetString("cbtWLDTrendZero.Strings.NoColor");
            //this.cbtWLDTrendZero.Strings.RecentColors = resources.GetString("cbtWLDTrendZero.Strings.RecentColors");
            //this.cbtWLDTrendZero.Strings.StandardColors = resources.GetString("cbtWLDTrendZero.Strings.StandardColors");
            //this.cbtWLDTrendZero.Strings.ThemeColors = resources.GetString("cbtWLDTrendZero.Strings.ThemeColors");
            //this.cbtWLDTrendZero.Values.ExtraText = resources.GetString("cbtWLDTrendZero.Values.ExtraText");
            //this.cbtWLDTrendZero.Values.Image = ((System.Drawing.Image)(resources.GetObject("cbtWLDTrendZero.Values.Image")));
            //this.cbtWLDTrendZero.Values.ImageStates.ImageCheckedNormal = null;
            //this.cbtWLDTrendZero.Values.ImageStates.ImageCheckedPressed = null;
            //this.cbtWLDTrendZero.Values.ImageStates.ImageCheckedTracking = null;
            //this.cbtWLDTrendZero.Values.ImageStates.ImageDisabled = null;
            //this.cbtWLDTrendZero.Values.ImageStates.ImageNormal = null;
            //this.cbtWLDTrendZero.Values.ImageStates.ImagePressed = null;
            //this.cbtWLDTrendZero.Values.ImageStates.ImageTracking = null;
            //this.cbtWLDTrendZero.Values.ImageTransparentColor = ((System.Drawing.Color)(resources.GetObject("cbtWLDTrendZero.Values.ImageTransparentColor")));
            //this.cbtWLDTrendZero.Values.Text = resources.GetString("cbtWLDTrendZero.Values.Text");
            // 
            // lblWLDTrendZero
            // 
            this.lblWLDTrendZero.AccessibleDescription = null;
            this.lblWLDTrendZero.AccessibleName = null;
            resources.ApplyResources(this.lblWLDTrendZero, "lblWLDTrendZero");
            this.lblWLDTrendZero.Font = null;
            this.lblWLDTrendZero.Name = "lblWLDTrendZero";
            // 
            // cbtWLDTrendDraw
            // 
            this.cbtWLDTrendDraw.AccessibleDescription = null;
            this.cbtWLDTrendDraw.AccessibleName = null;
            resources.ApplyResources(this.cbtWLDTrendDraw, "cbtWLDTrendDraw");
            this.cbtWLDTrendDraw.BackgroundImage = null;
            //this.cbtWLDTrendDraw.ButtonStyle = ComponentFactory.Krypton.Toolkit.ButtonStyle.Standalone;
            this.cbtWLDTrendDraw.Font = null;
            this.cbtWLDTrendDraw.Name = "cbtWLDTrendDraw";
            //this.cbtWLDTrendDraw.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.ProfessionalSystem;
            //this.cbtWLDTrendDraw.SchemeThemes = ComponentFactory.Krypton.Toolkit.ColorScheme.OfficeStandard;
            //this.cbtWLDTrendDraw.Strings.MoreColors = resources.GetString("cbtWLDTrendDraw.Strings.MoreColors");
            //this.cbtWLDTrendDraw.Strings.NoColor = resources.GetString("cbtWLDTrendDraw.Strings.NoColor");
            //this.cbtWLDTrendDraw.Strings.RecentColors = resources.GetString("cbtWLDTrendDraw.Strings.RecentColors");
            //this.cbtWLDTrendDraw.Strings.StandardColors = resources.GetString("cbtWLDTrendDraw.Strings.StandardColors");
            //this.cbtWLDTrendDraw.Strings.ThemeColors = resources.GetString("cbtWLDTrendDraw.Strings.ThemeColors");
            //this.cbtWLDTrendDraw.Values.ExtraText = resources.GetString("cbtWLDTrendDraw.Values.ExtraText");
            //this.cbtWLDTrendDraw.Values.Image = ((System.Drawing.Image)(resources.GetObject("cbtWLDTrendDraw.Values.Image")));
            //this.cbtWLDTrendDraw.Values.ImageStates.ImageCheckedNormal = null;
            //this.cbtWLDTrendDraw.Values.ImageStates.ImageCheckedPressed = null;
            //this.cbtWLDTrendDraw.Values.ImageStates.ImageCheckedTracking = null;
            //this.cbtWLDTrendDraw.Values.ImageStates.ImageDisabled = null;
            //this.cbtWLDTrendDraw.Values.ImageStates.ImageNormal = null;
            //this.cbtWLDTrendDraw.Values.ImageStates.ImagePressed = null;
            //this.cbtWLDTrendDraw.Values.ImageStates.ImageTracking = null;
            //this.cbtWLDTrendDraw.Values.ImageTransparentColor = ((System.Drawing.Color)(resources.GetObject("cbtWLDTrendDraw.Values.ImageTransparentColor")));
            //this.cbtWLDTrendDraw.Values.Text = resources.GetString("cbtWLDTrendDraw.Values.Text");
            // 
            // lblWLDTrendDraw
            // 
            this.lblWLDTrendDraw.AccessibleDescription = null;
            this.lblWLDTrendDraw.AccessibleName = null;
            resources.ApplyResources(this.lblWLDTrendDraw, "lblWLDTrendDraw");
            this.lblWLDTrendDraw.Font = null;
            this.lblWLDTrendDraw.Name = "lblWLDTrendDraw";
            // 
            // cbtWLDTrendLoss
            // 
            this.cbtWLDTrendLoss.AccessibleDescription = null;
            this.cbtWLDTrendLoss.AccessibleName = null;
            resources.ApplyResources(this.cbtWLDTrendLoss, "cbtWLDTrendLoss");
            this.cbtWLDTrendLoss.BackgroundImage = null;
            //this.cbtWLDTrendLoss.ButtonStyle = ComponentFactory.Krypton.Toolkit.ButtonStyle.Standalone;
            this.cbtWLDTrendLoss.Font = null;
            this.cbtWLDTrendLoss.Name = "cbtWLDTrendLoss";
            //this.cbtWLDTrendLoss.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.ProfessionalSystem;
            //this.cbtWLDTrendLoss.SchemeThemes = ComponentFactory.Krypton.Toolkit.ColorScheme.OfficeStandard;
            //this.cbtWLDTrendLoss.Strings.MoreColors = resources.GetString("cbtWLDTrendLoss.Strings.MoreColors");
            //this.cbtWLDTrendLoss.Strings.NoColor = resources.GetString("cbtWLDTrendLoss.Strings.NoColor");
            //this.cbtWLDTrendLoss.Strings.RecentColors = resources.GetString("cbtWLDTrendLoss.Strings.RecentColors");
            //this.cbtWLDTrendLoss.Strings.StandardColors = resources.GetString("cbtWLDTrendLoss.Strings.StandardColors");
            //this.cbtWLDTrendLoss.Strings.ThemeColors = resources.GetString("cbtWLDTrendLoss.Strings.ThemeColors");
            //this.cbtWLDTrendLoss.Values.ExtraText = resources.GetString("cbtWLDTrendLoss.Values.ExtraText");
            //this.cbtWLDTrendLoss.Values.Image = ((System.Drawing.Image)(resources.GetObject("cbtWLDTrendLoss.Values.Image")));
            //this.cbtWLDTrendLoss.Values.ImageStates.ImageCheckedNormal = null;
            //this.cbtWLDTrendLoss.Values.ImageStates.ImageCheckedPressed = null;
            //this.cbtWLDTrendLoss.Values.ImageStates.ImageCheckedTracking = null;
            //this.cbtWLDTrendLoss.Values.ImageStates.ImageDisabled = null;
            //this.cbtWLDTrendLoss.Values.ImageStates.ImageNormal = null;
            //this.cbtWLDTrendLoss.Values.ImageStates.ImagePressed = null;
            //this.cbtWLDTrendLoss.Values.ImageStates.ImageTracking = null;
            //this.cbtWLDTrendLoss.Values.ImageTransparentColor = ((System.Drawing.Color)(resources.GetObject("cbtWLDTrendLoss.Values.ImageTransparentColor")));
            //this.cbtWLDTrendLoss.Values.Text = resources.GetString("cbtWLDTrendLoss.Values.Text");
            // 
            // lblWLDTrendLoss
            // 
            this.lblWLDTrendLoss.AccessibleDescription = null;
            this.lblWLDTrendLoss.AccessibleName = null;
            resources.ApplyResources(this.lblWLDTrendLoss, "lblWLDTrendLoss");
            this.lblWLDTrendLoss.Font = null;
            this.lblWLDTrendLoss.Name = "lblWLDTrendLoss";
            // 
            // cbtWLDTrendWin
            // 
            this.cbtWLDTrendWin.AccessibleDescription = null;
            this.cbtWLDTrendWin.AccessibleName = null;
            resources.ApplyResources(this.cbtWLDTrendWin, "cbtWLDTrendWin");
            this.cbtWLDTrendWin.BackgroundImage = null;
            //this.cbtWLDTrendWin.ButtonStyle = ComponentFactory.Krypton.Toolkit.ButtonStyle.Standalone;
            this.cbtWLDTrendWin.Font = null;
            this.cbtWLDTrendWin.Name = "cbtWLDTrendWin";
            //this.cbtWLDTrendWin.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.ProfessionalSystem;
            //this.cbtWLDTrendWin.SchemeThemes = ComponentFactory.Krypton.Toolkit.ColorScheme.OfficeStandard;
            //this.cbtWLDTrendWin.Strings.MoreColors = resources.GetString("cbtWLDTrendWin.Strings.MoreColors");
            //this.cbtWLDTrendWin.Strings.NoColor = resources.GetString("cbtWLDTrendWin.Strings.NoColor");
            //this.cbtWLDTrendWin.Strings.RecentColors = resources.GetString("cbtWLDTrendWin.Strings.RecentColors");
            //this.cbtWLDTrendWin.Strings.StandardColors = resources.GetString("cbtWLDTrendWin.Strings.StandardColors");
            //this.cbtWLDTrendWin.Strings.ThemeColors = resources.GetString("cbtWLDTrendWin.Strings.ThemeColors");
            //this.cbtWLDTrendWin.Values.ExtraText = resources.GetString("cbtWLDTrendWin.Values.ExtraText");
            //this.cbtWLDTrendWin.Values.Image = ((System.Drawing.Image)(resources.GetObject("cbtWLDTrendWin.Values.Image")));
            //this.cbtWLDTrendWin.Values.ImageStates.ImageCheckedNormal = null;
            //this.cbtWLDTrendWin.Values.ImageStates.ImageCheckedPressed = null;
            //this.cbtWLDTrendWin.Values.ImageStates.ImageCheckedTracking = null;
            //this.cbtWLDTrendWin.Values.ImageStates.ImageDisabled = null;
            //this.cbtWLDTrendWin.Values.ImageStates.ImageNormal = null;
            //this.cbtWLDTrendWin.Values.ImageStates.ImagePressed = null;
            //this.cbtWLDTrendWin.Values.ImageStates.ImageTracking = null;
            //this.cbtWLDTrendWin.Values.ImageTransparentColor = ((System.Drawing.Color)(resources.GetObject("cbtWLDTrendWin.Values.ImageTransparentColor")));
            //this.cbtWLDTrendWin.Values.Text = resources.GetString("cbtWLDTrendWin.Values.Text");
            // 
            // lblWLDTrendWin
            // 
            this.lblWLDTrendWin.AccessibleDescription = null;
            this.lblWLDTrendWin.AccessibleName = null;
            resources.ApplyResources(this.lblWLDTrendWin, "lblWLDTrendWin");
            this.lblWLDTrendWin.Font = null;
            this.lblWLDTrendWin.Name = "lblWLDTrendWin";
            // 
            // tbpOverUnder
            // 
            this.tbpOverUnder.AccessibleDescription = null;
            this.tbpOverUnder.AccessibleName = null;
            resources.ApplyResources(this.tbpOverUnder, "tbpOverUnder");
            this.tbpOverUnder.BackgroundImage = null;
            this.tbpOverUnder.Controls.Add(this.gbxOUUnderColorRanges);
            this.tbpOverUnder.Controls.Add(this.gbxOUOverColorRanges);
            this.tbpOverUnder.Controls.Add(this.gbxOUTrend);
            this.tbpOverUnder.Font = null;
            this.tbpOverUnder.Name = "tbpOverUnder";
            this.tbpOverUnder.UseVisualStyleBackColor = true;
            // 
            // gbxOUUnderColorRanges
            // 
            this.gbxOUUnderColorRanges.AccessibleDescription = null;
            this.gbxOUUnderColorRanges.AccessibleName = null;
            resources.ApplyResources(this.gbxOUUnderColorRanges, "gbxOUUnderColorRanges");
            this.gbxOUUnderColorRanges.BackgroundImage = null;
            this.gbxOUUnderColorRanges.Controls.Add(this.pnlOUUnderRanges);
            this.gbxOUUnderColorRanges.Controls.Add(this.pnlOUUnderRangeButton);
            this.gbxOUUnderColorRanges.Font = null;
            this.gbxOUUnderColorRanges.Name = "gbxOUUnderColorRanges";
            this.gbxOUUnderColorRanges.TabStop = false;
            // 
            // pnlOUUnderRanges
            // 
            this.pnlOUUnderRanges.AccessibleDescription = null;
            this.pnlOUUnderRanges.AccessibleName = null;
            resources.ApplyResources(this.pnlOUUnderRanges, "pnlOUUnderRanges");
            this.pnlOUUnderRanges.BackgroundImage = null;
            this.pnlOUUnderRanges.Font = null;
            this.pnlOUUnderRanges.Name = "pnlOUUnderRanges";
            // 
            // pnlOUUnderRangeButton
            // 
            this.pnlOUUnderRangeButton.AccessibleDescription = null;
            this.pnlOUUnderRangeButton.AccessibleName = null;
            resources.ApplyResources(this.pnlOUUnderRangeButton, "pnlOUUnderRangeButton");
            this.pnlOUUnderRangeButton.BackgroundImage = null;
            this.pnlOUUnderRangeButton.Controls.Add(this.btnOUUnderRangeNew);
            this.pnlOUUnderRangeButton.Font = null;
            this.pnlOUUnderRangeButton.Name = "pnlOUUnderRangeButton";
            // 
            // btnOUUnderRangeNew
            // 
            this.btnOUUnderRangeNew.AccessibleDescription = null;
            this.btnOUUnderRangeNew.AccessibleName = null;
            resources.ApplyResources(this.btnOUUnderRangeNew, "btnOUUnderRangeNew");
            this.btnOUUnderRangeNew.BackgroundImage = null;
            this.btnOUUnderRangeNew.Font = null;
            this.btnOUUnderRangeNew.Name = "btnOUUnderRangeNew";
            this.btnOUUnderRangeNew.UseVisualStyleBackColor = true;
            this.btnOUUnderRangeNew.Click += new System.EventHandler(this.btnOUUnderRangeNew_Click);
            // 
            // gbxOUOverColorRanges
            // 
            this.gbxOUOverColorRanges.AccessibleDescription = null;
            this.gbxOUOverColorRanges.AccessibleName = null;
            resources.ApplyResources(this.gbxOUOverColorRanges, "gbxOUOverColorRanges");
            this.gbxOUOverColorRanges.BackgroundImage = null;
            this.gbxOUOverColorRanges.Controls.Add(this.pnlOUOverRanges);
            this.gbxOUOverColorRanges.Controls.Add(this.pnlOUOverRangeButton);
            this.gbxOUOverColorRanges.Font = null;
            this.gbxOUOverColorRanges.Name = "gbxOUOverColorRanges";
            this.gbxOUOverColorRanges.TabStop = false;
            // 
            // pnlOUOverRanges
            // 
            this.pnlOUOverRanges.AccessibleDescription = null;
            this.pnlOUOverRanges.AccessibleName = null;
            resources.ApplyResources(this.pnlOUOverRanges, "pnlOUOverRanges");
            this.pnlOUOverRanges.BackgroundImage = null;
            this.pnlOUOverRanges.Font = null;
            this.pnlOUOverRanges.Name = "pnlOUOverRanges";
            // 
            // pnlOUOverRangeButton
            // 
            this.pnlOUOverRangeButton.AccessibleDescription = null;
            this.pnlOUOverRangeButton.AccessibleName = null;
            resources.ApplyResources(this.pnlOUOverRangeButton, "pnlOUOverRangeButton");
            this.pnlOUOverRangeButton.BackgroundImage = null;
            this.pnlOUOverRangeButton.Controls.Add(this.btnOUOverRangeNew);
            this.pnlOUOverRangeButton.Font = null;
            this.pnlOUOverRangeButton.Name = "pnlOUOverRangeButton";
            // 
            // btnOUOverRangeNew
            // 
            this.btnOUOverRangeNew.AccessibleDescription = null;
            this.btnOUOverRangeNew.AccessibleName = null;
            resources.ApplyResources(this.btnOUOverRangeNew, "btnOUOverRangeNew");
            this.btnOUOverRangeNew.BackgroundImage = null;
            this.btnOUOverRangeNew.Font = null;
            this.btnOUOverRangeNew.Name = "btnOUOverRangeNew";
            this.btnOUOverRangeNew.UseVisualStyleBackColor = true;
            this.btnOUOverRangeNew.Click += new System.EventHandler(this.btnOUOverRangeNew_Click);
            // 
            // gbxOUTrend
            // 
            this.gbxOUTrend.AccessibleDescription = null;
            this.gbxOUTrend.AccessibleName = null;
            resources.ApplyResources(this.gbxOUTrend, "gbxOUTrend");
            this.gbxOUTrend.BackgroundImage = null;
            this.gbxOUTrend.Controls.Add(this.cbtOUTrendUnder);
            this.gbxOUTrend.Controls.Add(this.lblUnder);
            this.gbxOUTrend.Controls.Add(this.cbtOUTrendOver);
            this.gbxOUTrend.Controls.Add(this.lblOver);
            this.gbxOUTrend.Font = null;
            this.gbxOUTrend.Name = "gbxOUTrend";
            this.gbxOUTrend.TabStop = false;
            // 
            // cbtOUTrendUnder
            // 
            this.cbtOUTrendUnder.AccessibleDescription = null;
            this.cbtOUTrendUnder.AccessibleName = null;
            resources.ApplyResources(this.cbtOUTrendUnder, "cbtOUTrendUnder");
            this.cbtOUTrendUnder.BackgroundImage = null;
            //this.cbtOUTrendUnder.ButtonStyle = ComponentFactory.Krypton.Toolkit.ButtonStyle.Standalone;
            this.cbtOUTrendUnder.Font = null;
            this.cbtOUTrendUnder.Name = "cbtOUTrendUnder";
            //this.cbtOUTrendUnder.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.ProfessionalSystem;
            //this.cbtOUTrendUnder.SchemeThemes = ComponentFactory.Krypton.Toolkit.ColorScheme.OfficeStandard;
            //this.cbtOUTrendUnder.Strings.MoreColors = resources.GetString("cbtOUTrendUnder.Strings.MoreColors");
            //this.cbtOUTrendUnder.Strings.NoColor = resources.GetString("cbtOUTrendUnder.Strings.NoColor");
            //this.cbtOUTrendUnder.Strings.RecentColors = resources.GetString("cbtOUTrendUnder.Strings.RecentColors");
            //this.cbtOUTrendUnder.Strings.StandardColors = resources.GetString("cbtOUTrendUnder.Strings.StandardColors");
            //this.cbtOUTrendUnder.Strings.ThemeColors = resources.GetString("cbtOUTrendUnder.Strings.ThemeColors");
            //this.cbtOUTrendUnder.Values.ExtraText = resources.GetString("cbtOUTrendUnder.Values.ExtraText");
            //this.cbtOUTrendUnder.Values.Image = ((System.Drawing.Image)(resources.GetObject("cbtOUTrendUnder.Values.Image")));
            //this.cbtOUTrendUnder.Values.ImageStates.ImageCheckedNormal = null;
            //this.cbtOUTrendUnder.Values.ImageStates.ImageCheckedPressed = null;
            //this.cbtOUTrendUnder.Values.ImageStates.ImageCheckedTracking = null;
            //this.cbtOUTrendUnder.Values.ImageStates.ImageDisabled = null;
            //this.cbtOUTrendUnder.Values.ImageStates.ImageNormal = null;
            //this.cbtOUTrendUnder.Values.ImageStates.ImagePressed = null;
            //this.cbtOUTrendUnder.Values.ImageStates.ImageTracking = null;
            //this.cbtOUTrendUnder.Values.ImageTransparentColor = ((System.Drawing.Color)(resources.GetObject("cbtOUTrendUnder.Values.ImageTransparentColor")));
            //this.cbtOUTrendUnder.Values.Text = resources.GetString("cbtOUTrendUnder.Values.Text");
            //this.cbtOUTrendUnder.SelectedColorChanged += new System.EventHandler<ComponentFactory.Krypton.Toolkit.ColorEventArgs>(this.cbtOUTrendUnder_SelectedColorChanged);
            // 
            // lblUnder
            // 
            this.lblUnder.AccessibleDescription = null;
            this.lblUnder.AccessibleName = null;
            resources.ApplyResources(this.lblUnder, "lblUnder");
            this.lblUnder.Font = null;
            this.lblUnder.Name = "lblUnder";
            // 
            // cbtOUTrendOver
            // 
            this.cbtOUTrendOver.AccessibleDescription = null;
            this.cbtOUTrendOver.AccessibleName = null;
            resources.ApplyResources(this.cbtOUTrendOver, "cbtOUTrendOver");
            this.cbtOUTrendOver.BackgroundImage = null;
            //this.cbtOUTrendOver.ButtonStyle = ComponentFactory.Krypton.Toolkit.ButtonStyle.Standalone;
            this.cbtOUTrendOver.Font = null;
            this.cbtOUTrendOver.Name = "cbtOUTrendOver";
            //this.cbtOUTrendOver.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.ProfessionalSystem;
            //this.cbtOUTrendOver.SchemeThemes = ComponentFactory.Krypton.Toolkit.ColorScheme.OfficeStandard;
            //this.cbtOUTrendOver.Strings.MoreColors = resources.GetString("cbtOUTrendOver.Strings.MoreColors");
            //this.cbtOUTrendOver.Strings.NoColor = resources.GetString("cbtOUTrendOver.Strings.NoColor");
            //this.cbtOUTrendOver.Strings.RecentColors = resources.GetString("cbtOUTrendOver.Strings.RecentColors");
            //this.cbtOUTrendOver.Strings.StandardColors = resources.GetString("cbtOUTrendOver.Strings.StandardColors");
            //this.cbtOUTrendOver.Strings.ThemeColors = resources.GetString("cbtOUTrendOver.Strings.ThemeColors");
            //this.cbtOUTrendOver.Values.ExtraText = resources.GetString("cbtOUTrendOver.Values.ExtraText");
            //this.cbtOUTrendOver.Values.Image = ((System.Drawing.Image)(resources.GetObject("cbtOUTrendOver.Values.Image")));
            //this.cbtOUTrendOver.Values.ImageStates.ImageCheckedNormal = null;
            //this.cbtOUTrendOver.Values.ImageStates.ImageCheckedPressed = null;
            //this.cbtOUTrendOver.Values.ImageStates.ImageCheckedTracking = null;
            //this.cbtOUTrendOver.Values.ImageStates.ImageDisabled = null;
            //this.cbtOUTrendOver.Values.ImageStates.ImageNormal = null;
            //this.cbtOUTrendOver.Values.ImageStates.ImagePressed = null;
            //this.cbtOUTrendOver.Values.ImageStates.ImageTracking = null;
            //this.cbtOUTrendOver.Values.ImageTransparentColor = ((System.Drawing.Color)(resources.GetObject("cbtOUTrendOver.Values.ImageTransparentColor")));
            //this.cbtOUTrendOver.Values.Text = resources.GetString("cbtOUTrendOver.Values.Text");
            //this.cbtOUTrendOver.SelectedColorChanged += new System.EventHandler<ComponentFactory.Krypton.Toolkit.ColorEventArgs>(this.cbtOUTrendOver_SelectedColorChanged);
            // 
            // lblOver
            // 
            this.lblOver.AccessibleDescription = null;
            this.lblOver.AccessibleName = null;
            resources.ApplyResources(this.lblOver, "lblOver");
            this.lblOver.Font = null;
            this.lblOver.Name = "lblOver";
            // 
            // tbpGameColor
            // 
            this.tbpGameColor.AccessibleDescription = null;
            this.tbpGameColor.AccessibleName = null;
            resources.ApplyResources(this.tbpGameColor, "tbpGameColor");
            this.tbpGameColor.BackgroundImage = null;
            this.tbpGameColor.Controls.Add(this.pnlStatColors);
            this.tbpGameColor.Controls.Add(this.pnlButtons);
            this.tbpGameColor.Font = null;
            this.tbpGameColor.Name = "tbpGameColor";
            this.tbpGameColor.UseVisualStyleBackColor = true;
            // 
            // pnlStatColors
            // 
            this.pnlStatColors.AccessibleDescription = null;
            this.pnlStatColors.AccessibleName = null;
            resources.ApplyResources(this.pnlStatColors, "pnlStatColors");
            this.pnlStatColors.BackgroundImage = null;
            this.pnlStatColors.Font = null;
            this.pnlStatColors.Name = "pnlStatColors";
            // 
            // pnlButtons
            // 
            this.pnlButtons.AccessibleDescription = null;
            this.pnlButtons.AccessibleName = null;
            resources.ApplyResources(this.pnlButtons, "pnlButtons");
            this.pnlButtons.BackgroundImage = null;
            this.pnlButtons.Controls.Add(this.btnNewColorStat);
            this.pnlButtons.Font = null;
            this.pnlButtons.Name = "pnlButtons";
            // 
            // btnNewColorStat
            // 
            this.btnNewColorStat.AccessibleDescription = null;
            this.btnNewColorStat.AccessibleName = null;
            resources.ApplyResources(this.btnNewColorStat, "btnNewColorStat");
            this.btnNewColorStat.BackgroundImage = null;
            this.btnNewColorStat.Font = null;
            this.btnNewColorStat.Name = "btnNewColorStat";
            this.btnNewColorStat.UseVisualStyleBackColor = true;
            this.btnNewColorStat.Click += new System.EventHandler(this.btnNewColorStat_Click);
            // 
            // ctlConfiguration
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.Controls.Add(this.tbcConfig);
            this.Font = null;
            this.Name = "ctlConfiguration";
            this.tbcConfig.ResumeLayout(false);
            this.tbpGeneral.ResumeLayout(false);
            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spnAgeOfData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnHistoricData)).EndInit();
            this.tbpScoreMatrix.ResumeLayout(false);
            this.pnlNewColorRange.ResumeLayout(false);
            this.pnlNewColorRange.PerformLayout();
            this.tbpWLDTrend.ResumeLayout(false);
            this.gbxWLDDrawRangeColors.ResumeLayout(false);
            this.pnlWLDDrawRangeButton.ResumeLayout(false);
            this.pnlWLDDrawRangeButton.PerformLayout();
            this.gbxWLDLossRangeColors.ResumeLayout(false);
            this.pnlWLDLossRangeButton.ResumeLayout(false);
            this.pnlWLDLossRangeButton.PerformLayout();
            this.gbxWLDWinRangeColors.ResumeLayout(false);
            this.pnlWLDWinRangeButton.ResumeLayout(false);
            this.pnlWLDWinRangeButton.PerformLayout();
            this.gbxWLDTrendColors.ResumeLayout(false);
            this.gbxWLDTrendColors.PerformLayout();
            this.tbpOverUnder.ResumeLayout(false);
            this.gbxOUUnderColorRanges.ResumeLayout(false);
            this.pnlOUUnderRangeButton.ResumeLayout(false);
            this.pnlOUUnderRangeButton.PerformLayout();
            this.gbxOUOverColorRanges.ResumeLayout(false);
            this.pnlOUOverRangeButton.ResumeLayout(false);
            this.pnlOUOverRangeButton.PerformLayout();
            this.gbxOUTrend.ResumeLayout(false);
            this.gbxOUTrend.PerformLayout();
            this.tbpGameColor.ResumeLayout(false);
            this.pnlButtons.ResumeLayout(false);
            this.pnlButtons.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tbcConfig;
        private System.Windows.Forms.TabPage tbpGeneral;
        private System.Windows.Forms.TabPage tbpScoreMatrix;
        private System.Windows.Forms.Panel pnlColorRanges;
        private System.Windows.Forms.Panel pnlNewColorRange;
        private System.Windows.Forms.Button btnNewRange;
        private System.Windows.Forms.TabPage tbpWLDTrend;
        private System.Windows.Forms.GroupBox gbxWLDTrendColors;
        private System.Windows.Forms.Label lblWLDTrendWin;
        private net.sxtrader.muk.ColorButton cbtWLDTrendWin;
        private net.sxtrader.muk.ColorButton cbtWLDTrendLoss;
        private System.Windows.Forms.Label lblWLDTrendLoss;
        private net.sxtrader.muk.ColorButton cbtWLDTrendDraw;
        private System.Windows.Forms.Label lblWLDTrendDraw;
        private net.sxtrader.muk.ColorButton cbtWLDTrendZero;
        private System.Windows.Forms.Label lblWLDTrendZero;
        private System.Windows.Forms.GroupBox gbxWLDWinRangeColors;
        private System.Windows.Forms.Panel pnlWLDWinRanges;
        private System.Windows.Forms.Panel pnlWLDWinRangeButton;
        private System.Windows.Forms.Button btnWLDWinRangeNew;
        private System.Windows.Forms.GroupBox gbxWLDLossRangeColors;
        private System.Windows.Forms.Panel pnlWLDLossRanges;
        private System.Windows.Forms.Panel pnlWLDLossRangeButton;
        private System.Windows.Forms.Button btnWLDLossRangeNew;
        private System.Windows.Forms.GroupBox gbxWLDDrawRangeColors;
        private System.Windows.Forms.Panel pnlWLDDrawRanges;
        private System.Windows.Forms.Panel pnlWLDDrawRangeButton;
        private System.Windows.Forms.Button btnWLDDrawRangeNew;
        private System.Windows.Forms.TabPage tbpOverUnder;
        private System.Windows.Forms.GroupBox gbxOUTrend;
        private net.sxtrader.muk.ColorButton cbtOUTrendUnder;
        private System.Windows.Forms.Label lblUnder;
        private net.sxtrader.muk.ColorButton cbtOUTrendOver;
        private System.Windows.Forms.Label lblOver;
        private System.Windows.Forms.GroupBox gbxOUOverColorRanges;
        private System.Windows.Forms.Panel pnlOUOverRanges;
        private System.Windows.Forms.Panel pnlOUOverRangeButton;
        private System.Windows.Forms.Button btnOUOverRangeNew;
        private System.Windows.Forms.GroupBox gbxOUUnderColorRanges;
        private System.Windows.Forms.Panel pnlOUUnderRanges;
        private System.Windows.Forms.Panel pnlOUUnderRangeButton;
        private System.Windows.Forms.Button btnOUUnderRangeNew;
        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.NumericUpDown spnHistoricData;
        private System.Windows.Forms.Label lblNoHistoricData;
        private System.Windows.Forms.TabPage tbpGameColor;
        private System.Windows.Forms.Panel pnlStatColors;
        private System.Windows.Forms.Panel pnlButtons;
        private System.Windows.Forms.Button btnNewColorStat;
        private System.Windows.Forms.NumericUpDown spnAgeOfData;
        private System.Windows.Forms.Label lblAgeOfData;
    }
}
