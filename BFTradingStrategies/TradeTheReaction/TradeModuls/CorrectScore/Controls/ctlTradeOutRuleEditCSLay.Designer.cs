namespace net.sxtrader.bftradingstrategies.ttr.TradeModuls.CorrectScore.Controls
{
    partial class ctlTradeOutRuleEditCSLay
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ctlTradeOutRuleEditCSLay));
            this.gbxTrigger = new System.Windows.Forms.GroupBox();
            this.spnCheckOrder = new System.Windows.Forms.NumericUpDown();
            this.lblCheckOder = new System.Windows.Forms.Label();
            this.cbxTrigger = new System.Windows.Forms.ComboBox();
            this.lblTrigger = new System.Windows.Forms.Label();
            this.gbxCheckRules = new System.Windows.Forms.GroupBox();
            this.btnScores = new System.Windows.Forms.Button();
            this.chkScore = new System.Windows.Forms.CheckBox();
            this.rbnRCEqual = new System.Windows.Forms.RadioButton();
            this.rbnRCTeamB = new System.Windows.Forms.RadioButton();
            this.rbnRCTeamA = new System.Windows.Forms.RadioButton();
            this.rbnNoRedCard = new System.Windows.Forms.RadioButton();
            this.spnPlaytimeHi = new System.Windows.Forms.NumericUpDown();
            this.lblAnd1 = new System.Windows.Forms.Label();
            this.spnPlaytimeLo = new System.Windows.Forms.NumericUpDown();
            this.lblBetween1 = new System.Windows.Forms.Label();
            this.chkCheckPlaytime = new System.Windows.Forms.CheckBox();
            this.gbxSettings = new System.Windows.Forms.GroupBox();
            this.pnlButtons = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnNew = new System.Windows.Forms.Button();
            this.chkNoTrade = new System.Windows.Forms.CheckBox();
            this.gbxTrigger.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spnCheckOrder)).BeginInit();
            this.gbxCheckRules.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spnPlaytimeHi)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnPlaytimeLo)).BeginInit();
            this.gbxSettings.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            this.SuspendLayout();
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
            this.gbxCheckRules.Controls.Add(this.btnScores);
            this.gbxCheckRules.Controls.Add(this.chkScore);
            this.gbxCheckRules.Controls.Add(this.rbnRCEqual);
            this.gbxCheckRules.Controls.Add(this.rbnRCTeamB);
            this.gbxCheckRules.Controls.Add(this.rbnRCTeamA);
            this.gbxCheckRules.Controls.Add(this.rbnNoRedCard);
            this.gbxCheckRules.Controls.Add(this.spnPlaytimeHi);
            this.gbxCheckRules.Controls.Add(this.lblAnd1);
            this.gbxCheckRules.Controls.Add(this.spnPlaytimeLo);
            this.gbxCheckRules.Controls.Add(this.lblBetween1);
            this.gbxCheckRules.Controls.Add(this.chkCheckPlaytime);
            resources.ApplyResources(this.gbxCheckRules, "gbxCheckRules");
            this.gbxCheckRules.Name = "gbxCheckRules";
            this.gbxCheckRules.TabStop = false;
            // 
            // btnScores
            // 
            resources.ApplyResources(this.btnScores, "btnScores");
            this.btnScores.Name = "btnScores";
            this.btnScores.UseVisualStyleBackColor = true;
            this.btnScores.Click += new System.EventHandler(this.btnScores_Click);
            // 
            // chkScore
            // 
            resources.ApplyResources(this.chkScore, "chkScore");
            this.chkScore.Name = "chkScore";
            this.chkScore.UseVisualStyleBackColor = true;
            this.chkScore.CheckedChanged += new System.EventHandler(this.chkScore_CheckedChanged);
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
            // spnPlaytimeHi
            // 
            resources.ApplyResources(this.spnPlaytimeHi, "spnPlaytimeHi");
            this.spnPlaytimeHi.Maximum = new decimal(new int[] {
            90,
            0,
            0,
            0});
            this.spnPlaytimeHi.Name = "spnPlaytimeHi";
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
            this.gbxSettings.Controls.Add(this.pnlButtons);
            this.gbxSettings.Controls.Add(this.chkNoTrade);
            resources.ApplyResources(this.gbxSettings, "gbxSettings");
            this.gbxSettings.Name = "gbxSettings";
            this.gbxSettings.TabStop = false;
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
            // chkNoTrade
            // 
            resources.ApplyResources(this.chkNoTrade, "chkNoTrade");
            this.chkNoTrade.Name = "chkNoTrade";
            this.chkNoTrade.UseVisualStyleBackColor = true;
            // 
            // ctlTradeOutRuleEditCSLay
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gbxSettings);
            this.Controls.Add(this.gbxCheckRules);
            this.Controls.Add(this.gbxTrigger);
            this.Name = "ctlTradeOutRuleEditCSLay";
            this.gbxTrigger.ResumeLayout(false);
            this.gbxTrigger.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spnCheckOrder)).EndInit();
            this.gbxCheckRules.ResumeLayout(false);
            this.gbxCheckRules.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spnPlaytimeHi)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnPlaytimeLo)).EndInit();
            this.gbxSettings.ResumeLayout(false);
            this.gbxSettings.PerformLayout();
            this.pnlButtons.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbxTrigger;
        private System.Windows.Forms.NumericUpDown spnCheckOrder;
        private System.Windows.Forms.Label lblCheckOder;
        private System.Windows.Forms.ComboBox cbxTrigger;
        private System.Windows.Forms.Label lblTrigger;
        private System.Windows.Forms.GroupBox gbxCheckRules;
        private System.Windows.Forms.Button btnScores;
        private System.Windows.Forms.CheckBox chkScore;
        private System.Windows.Forms.RadioButton rbnRCEqual;
        private System.Windows.Forms.RadioButton rbnRCTeamB;
        private System.Windows.Forms.RadioButton rbnRCTeamA;
        private System.Windows.Forms.RadioButton rbnNoRedCard;
        private System.Windows.Forms.NumericUpDown spnPlaytimeHi;
        private System.Windows.Forms.Label lblAnd1;
        private System.Windows.Forms.NumericUpDown spnPlaytimeLo;
        private System.Windows.Forms.Label lblBetween1;
        private System.Windows.Forms.CheckBox chkCheckPlaytime;
        private System.Windows.Forms.GroupBox gbxSettings;
        private System.Windows.Forms.CheckBox chkNoTrade;
        private System.Windows.Forms.Panel pnlButtons;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnNew;

    }
}
