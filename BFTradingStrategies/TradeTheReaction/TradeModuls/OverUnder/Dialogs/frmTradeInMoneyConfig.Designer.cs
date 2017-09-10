namespace net.sxtrader.bftradingstrategies.ttr.TradeModuls.OverUnder.Dialogs
{
    partial class frmTradeInMoneyConfig
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmTradeInMoneyConfig));
            this.lblPercentOf = new System.Windows.Forms.Label();
            this.spnFastBetPercentValue = new System.Windows.Forms.NumericUpDown();
            this.lblFastBetCurrency = new System.Windows.Forms.Label();
            this.spnFastBetFixedValue = new System.Windows.Forms.NumericUpDown();
            this.gbxPercentageRef = new System.Windows.Forms.GroupBox();
            this.rbnPercentageAvailibleAmount = new System.Windows.Forms.RadioButton();
            this.rbnPercentageTotalAmount = new System.Windows.Forms.RadioButton();
            this.gbxBetAmount = new System.Windows.Forms.GroupBox();
            this.gbxTargetSize = new System.Windows.Forms.GroupBox();
            this.lblRelativeTargetString = new System.Windows.Forms.Label();
            this.spnRelativeTargetAmount = new System.Windows.Forms.NumericUpDown();
            this.rbnRelativeTargetAmount = new System.Windows.Forms.RadioButton();
            this.rbnFixedTargetAmount = new System.Windows.Forms.RadioButton();
            this.lblCurrency2 = new System.Windows.Forms.Label();
            this.spnFixedTargetAmount = new System.Windows.Forms.NumericUpDown();
            this.rbnBetAmountByTarget = new System.Windows.Forms.RadioButton();
            this.cbxRelativeTradeType = new System.Windows.Forms.ComboBox();
            this.lblOf = new System.Windows.Forms.Label();
            this.cbxRelativeBetType = new System.Windows.Forms.ComboBox();
            this.lblPercentOf2 = new System.Windows.Forms.Label();
            this.spnRelativeBetSize = new System.Windows.Forms.NumericUpDown();
            this.rbnRelativeBetAmount = new System.Windows.Forms.RadioButton();
            this.rbnPercentBetAmount = new System.Windows.Forms.RadioButton();
            this.rbnFixedBetAmount = new System.Windows.Forms.RadioButton();
            this.pnlButtons = new System.Windows.Forms.Panel();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.gbxBetOdds = new System.Windows.Forms.GroupBox();
            this.lblTicksExplain = new System.Windows.Forms.Label();
            this.lblTicks = new System.Windows.Forms.Label();
            this.spnTicks = new System.Windows.Forms.NumericUpDown();
            this.rbnChangeOdds = new System.Windows.Forms.RadioButton();
            this.rbnGivenOdds = new System.Windows.Forms.RadioButton();
            this.gbxBetBehaviour = new System.Windows.Forms.GroupBox();
            this.chkKeepInplay = new System.Windows.Forms.CheckBox();
            this.chkKeepUnmatched = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.spnFastBetPercentValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnFastBetFixedValue)).BeginInit();
            this.gbxPercentageRef.SuspendLayout();
            this.gbxBetAmount.SuspendLayout();
            this.gbxTargetSize.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spnRelativeTargetAmount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnFixedTargetAmount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnRelativeBetSize)).BeginInit();
            this.pnlButtons.SuspendLayout();
            this.gbxBetOdds.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spnTicks)).BeginInit();
            this.gbxBetBehaviour.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblPercentOf
            // 
            this.lblPercentOf.AccessibleDescription = null;
            this.lblPercentOf.AccessibleName = null;
            resources.ApplyResources(this.lblPercentOf, "lblPercentOf");
            this.lblPercentOf.Font = null;
            this.lblPercentOf.Name = "lblPercentOf";
            // 
            // spnFastBetPercentValue
            // 
            this.spnFastBetPercentValue.AccessibleDescription = null;
            this.spnFastBetPercentValue.AccessibleName = null;
            resources.ApplyResources(this.spnFastBetPercentValue, "spnFastBetPercentValue");
            this.spnFastBetPercentValue.Font = null;
            this.spnFastBetPercentValue.Name = "spnFastBetPercentValue";
            // 
            // lblFastBetCurrency
            // 
            this.lblFastBetCurrency.AccessibleDescription = null;
            this.lblFastBetCurrency.AccessibleName = null;
            resources.ApplyResources(this.lblFastBetCurrency, "lblFastBetCurrency");
            this.lblFastBetCurrency.Font = null;
            this.lblFastBetCurrency.Name = "lblFastBetCurrency";
            // 
            // spnFastBetFixedValue
            // 
            this.spnFastBetFixedValue.AccessibleDescription = null;
            this.spnFastBetFixedValue.AccessibleName = null;
            resources.ApplyResources(this.spnFastBetFixedValue, "spnFastBetFixedValue");
            this.spnFastBetFixedValue.DecimalPlaces = 2;
            this.spnFastBetFixedValue.Font = null;
            this.spnFastBetFixedValue.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.spnFastBetFixedValue.Name = "spnFastBetFixedValue";
            // 
            // gbxPercentageRef
            // 
            this.gbxPercentageRef.AccessibleDescription = null;
            this.gbxPercentageRef.AccessibleName = null;
            resources.ApplyResources(this.gbxPercentageRef, "gbxPercentageRef");
            this.gbxPercentageRef.BackgroundImage = null;
            this.gbxPercentageRef.Controls.Add(this.rbnPercentageAvailibleAmount);
            this.gbxPercentageRef.Controls.Add(this.rbnPercentageTotalAmount);
            this.gbxPercentageRef.Font = null;
            this.gbxPercentageRef.Name = "gbxPercentageRef";
            this.gbxPercentageRef.TabStop = false;
            // 
            // rbnPercentageAvailibleAmount
            // 
            this.rbnPercentageAvailibleAmount.AccessibleDescription = null;
            this.rbnPercentageAvailibleAmount.AccessibleName = null;
            resources.ApplyResources(this.rbnPercentageAvailibleAmount, "rbnPercentageAvailibleAmount");
            this.rbnPercentageAvailibleAmount.BackgroundImage = null;
            this.rbnPercentageAvailibleAmount.Font = null;
            this.rbnPercentageAvailibleAmount.Name = "rbnPercentageAvailibleAmount";
            this.rbnPercentageAvailibleAmount.TabStop = true;
            this.rbnPercentageAvailibleAmount.UseVisualStyleBackColor = true;
            // 
            // rbnPercentageTotalAmount
            // 
            this.rbnPercentageTotalAmount.AccessibleDescription = null;
            this.rbnPercentageTotalAmount.AccessibleName = null;
            resources.ApplyResources(this.rbnPercentageTotalAmount, "rbnPercentageTotalAmount");
            this.rbnPercentageTotalAmount.BackgroundImage = null;
            this.rbnPercentageTotalAmount.Font = null;
            this.rbnPercentageTotalAmount.Name = "rbnPercentageTotalAmount";
            this.rbnPercentageTotalAmount.TabStop = true;
            this.rbnPercentageTotalAmount.UseVisualStyleBackColor = true;
            // 
            // gbxBetAmount
            // 
            this.gbxBetAmount.AccessibleDescription = null;
            this.gbxBetAmount.AccessibleName = null;
            resources.ApplyResources(this.gbxBetAmount, "gbxBetAmount");
            this.gbxBetAmount.BackgroundImage = null;
            this.gbxBetAmount.Controls.Add(this.gbxTargetSize);
            this.gbxBetAmount.Controls.Add(this.rbnBetAmountByTarget);
            this.gbxBetAmount.Controls.Add(this.cbxRelativeTradeType);
            this.gbxBetAmount.Controls.Add(this.lblOf);
            this.gbxBetAmount.Controls.Add(this.cbxRelativeBetType);
            this.gbxBetAmount.Controls.Add(this.lblPercentOf2);
            this.gbxBetAmount.Controls.Add(this.spnRelativeBetSize);
            this.gbxBetAmount.Controls.Add(this.rbnRelativeBetAmount);
            this.gbxBetAmount.Controls.Add(this.gbxPercentageRef);
            this.gbxBetAmount.Controls.Add(this.lblPercentOf);
            this.gbxBetAmount.Controls.Add(this.spnFastBetPercentValue);
            this.gbxBetAmount.Controls.Add(this.rbnPercentBetAmount);
            this.gbxBetAmount.Controls.Add(this.lblFastBetCurrency);
            this.gbxBetAmount.Controls.Add(this.spnFastBetFixedValue);
            this.gbxBetAmount.Controls.Add(this.rbnFixedBetAmount);
            this.gbxBetAmount.Font = null;
            this.gbxBetAmount.Name = "gbxBetAmount";
            this.gbxBetAmount.TabStop = false;
            // 
            // gbxTargetSize
            // 
            this.gbxTargetSize.AccessibleDescription = null;
            this.gbxTargetSize.AccessibleName = null;
            resources.ApplyResources(this.gbxTargetSize, "gbxTargetSize");
            this.gbxTargetSize.BackgroundImage = null;
            this.gbxTargetSize.Controls.Add(this.lblRelativeTargetString);
            this.gbxTargetSize.Controls.Add(this.spnRelativeTargetAmount);
            this.gbxTargetSize.Controls.Add(this.rbnRelativeTargetAmount);
            this.gbxTargetSize.Controls.Add(this.rbnFixedTargetAmount);
            this.gbxTargetSize.Controls.Add(this.lblCurrency2);
            this.gbxTargetSize.Controls.Add(this.spnFixedTargetAmount);
            this.gbxTargetSize.Font = null;
            this.gbxTargetSize.Name = "gbxTargetSize";
            this.gbxTargetSize.TabStop = false;
            // 
            // lblRelativeTargetString
            // 
            this.lblRelativeTargetString.AccessibleDescription = null;
            this.lblRelativeTargetString.AccessibleName = null;
            resources.ApplyResources(this.lblRelativeTargetString, "lblRelativeTargetString");
            this.lblRelativeTargetString.Font = null;
            this.lblRelativeTargetString.Name = "lblRelativeTargetString";
            // 
            // spnRelativeTargetAmount
            // 
            this.spnRelativeTargetAmount.AccessibleDescription = null;
            this.spnRelativeTargetAmount.AccessibleName = null;
            resources.ApplyResources(this.spnRelativeTargetAmount, "spnRelativeTargetAmount");
            this.spnRelativeTargetAmount.Font = null;
            this.spnRelativeTargetAmount.Name = "spnRelativeTargetAmount";
            // 
            // rbnRelativeTargetAmount
            // 
            this.rbnRelativeTargetAmount.AccessibleDescription = null;
            this.rbnRelativeTargetAmount.AccessibleName = null;
            resources.ApplyResources(this.rbnRelativeTargetAmount, "rbnRelativeTargetAmount");
            this.rbnRelativeTargetAmount.BackgroundImage = null;
            this.rbnRelativeTargetAmount.Font = null;
            this.rbnRelativeTargetAmount.Name = "rbnRelativeTargetAmount";
            this.rbnRelativeTargetAmount.TabStop = true;
            this.rbnRelativeTargetAmount.UseVisualStyleBackColor = true;
            // 
            // rbnFixedTargetAmount
            // 
            this.rbnFixedTargetAmount.AccessibleDescription = null;
            this.rbnFixedTargetAmount.AccessibleName = null;
            resources.ApplyResources(this.rbnFixedTargetAmount, "rbnFixedTargetAmount");
            this.rbnFixedTargetAmount.BackgroundImage = null;
            this.rbnFixedTargetAmount.Font = null;
            this.rbnFixedTargetAmount.Name = "rbnFixedTargetAmount";
            this.rbnFixedTargetAmount.TabStop = true;
            this.rbnFixedTargetAmount.UseVisualStyleBackColor = true;
            // 
            // lblCurrency2
            // 
            this.lblCurrency2.AccessibleDescription = null;
            this.lblCurrency2.AccessibleName = null;
            resources.ApplyResources(this.lblCurrency2, "lblCurrency2");
            this.lblCurrency2.Font = null;
            this.lblCurrency2.Name = "lblCurrency2";
            // 
            // spnFixedTargetAmount
            // 
            this.spnFixedTargetAmount.AccessibleDescription = null;
            this.spnFixedTargetAmount.AccessibleName = null;
            resources.ApplyResources(this.spnFixedTargetAmount, "spnFixedTargetAmount");
            this.spnFixedTargetAmount.DecimalPlaces = 2;
            this.spnFixedTargetAmount.Font = null;
            this.spnFixedTargetAmount.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.spnFixedTargetAmount.Name = "spnFixedTargetAmount";
            // 
            // rbnBetAmountByTarget
            // 
            this.rbnBetAmountByTarget.AccessibleDescription = null;
            this.rbnBetAmountByTarget.AccessibleName = null;
            resources.ApplyResources(this.rbnBetAmountByTarget, "rbnBetAmountByTarget");
            this.rbnBetAmountByTarget.BackgroundImage = null;
            this.rbnBetAmountByTarget.Font = null;
            this.rbnBetAmountByTarget.Name = "rbnBetAmountByTarget";
            this.rbnBetAmountByTarget.TabStop = true;
            this.rbnBetAmountByTarget.UseVisualStyleBackColor = true;
            // 
            // cbxRelativeTradeType
            // 
            this.cbxRelativeTradeType.AccessibleDescription = null;
            this.cbxRelativeTradeType.AccessibleName = null;
            resources.ApplyResources(this.cbxRelativeTradeType, "cbxRelativeTradeType");
            this.cbxRelativeTradeType.BackgroundImage = null;
            this.cbxRelativeTradeType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxRelativeTradeType.Font = null;
            this.cbxRelativeTradeType.FormattingEnabled = true;
            this.cbxRelativeTradeType.Name = "cbxRelativeTradeType";
            // 
            // lblOf
            // 
            this.lblOf.AccessibleDescription = null;
            this.lblOf.AccessibleName = null;
            resources.ApplyResources(this.lblOf, "lblOf");
            this.lblOf.Font = null;
            this.lblOf.Name = "lblOf";
            // 
            // cbxRelativeBetType
            // 
            this.cbxRelativeBetType.AccessibleDescription = null;
            this.cbxRelativeBetType.AccessibleName = null;
            resources.ApplyResources(this.cbxRelativeBetType, "cbxRelativeBetType");
            this.cbxRelativeBetType.BackgroundImage = null;
            this.cbxRelativeBetType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxRelativeBetType.Font = null;
            this.cbxRelativeBetType.FormattingEnabled = true;
            this.cbxRelativeBetType.Name = "cbxRelativeBetType";
            // 
            // lblPercentOf2
            // 
            this.lblPercentOf2.AccessibleDescription = null;
            this.lblPercentOf2.AccessibleName = null;
            resources.ApplyResources(this.lblPercentOf2, "lblPercentOf2");
            this.lblPercentOf2.Font = null;
            this.lblPercentOf2.Name = "lblPercentOf2";
            // 
            // spnRelativeBetSize
            // 
            this.spnRelativeBetSize.AccessibleDescription = null;
            this.spnRelativeBetSize.AccessibleName = null;
            resources.ApplyResources(this.spnRelativeBetSize, "spnRelativeBetSize");
            this.spnRelativeBetSize.Font = null;
            this.spnRelativeBetSize.Name = "spnRelativeBetSize";
            // 
            // rbnRelativeBetAmount
            // 
            this.rbnRelativeBetAmount.AccessibleDescription = null;
            this.rbnRelativeBetAmount.AccessibleName = null;
            resources.ApplyResources(this.rbnRelativeBetAmount, "rbnRelativeBetAmount");
            this.rbnRelativeBetAmount.BackgroundImage = null;
            this.rbnRelativeBetAmount.Font = null;
            this.rbnRelativeBetAmount.Name = "rbnRelativeBetAmount";
            this.rbnRelativeBetAmount.TabStop = true;
            this.rbnRelativeBetAmount.UseVisualStyleBackColor = true;
            // 
            // rbnPercentBetAmount
            // 
            this.rbnPercentBetAmount.AccessibleDescription = null;
            this.rbnPercentBetAmount.AccessibleName = null;
            resources.ApplyResources(this.rbnPercentBetAmount, "rbnPercentBetAmount");
            this.rbnPercentBetAmount.BackgroundImage = null;
            this.rbnPercentBetAmount.Font = null;
            this.rbnPercentBetAmount.Name = "rbnPercentBetAmount";
            this.rbnPercentBetAmount.TabStop = true;
            this.rbnPercentBetAmount.UseVisualStyleBackColor = true;
            // 
            // rbnFixedBetAmount
            // 
            this.rbnFixedBetAmount.AccessibleDescription = null;
            this.rbnFixedBetAmount.AccessibleName = null;
            resources.ApplyResources(this.rbnFixedBetAmount, "rbnFixedBetAmount");
            this.rbnFixedBetAmount.BackgroundImage = null;
            this.rbnFixedBetAmount.Font = null;
            this.rbnFixedBetAmount.Name = "rbnFixedBetAmount";
            this.rbnFixedBetAmount.TabStop = true;
            this.rbnFixedBetAmount.UseVisualStyleBackColor = true;
            // 
            // pnlButtons
            // 
            this.pnlButtons.AccessibleDescription = null;
            this.pnlButtons.AccessibleName = null;
            resources.ApplyResources(this.pnlButtons, "pnlButtons");
            this.pnlButtons.BackgroundImage = null;
            this.pnlButtons.Controls.Add(this.btnOK);
            this.pnlButtons.Controls.Add(this.btnCancel);
            this.pnlButtons.Font = null;
            this.pnlButtons.Name = "pnlButtons";
            // 
            // btnOK
            // 
            this.btnOK.AccessibleDescription = null;
            this.btnOK.AccessibleName = null;
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.BackgroundImage = null;
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Font = null;
            this.btnOK.Name = "btnOK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.AccessibleDescription = null;
            this.btnCancel.AccessibleName = null;
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.BackgroundImage = null;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Font = null;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // gbxBetOdds
            // 
            this.gbxBetOdds.AccessibleDescription = null;
            this.gbxBetOdds.AccessibleName = null;
            resources.ApplyResources(this.gbxBetOdds, "gbxBetOdds");
            this.gbxBetOdds.BackgroundImage = null;
            this.gbxBetOdds.Controls.Add(this.lblTicksExplain);
            this.gbxBetOdds.Controls.Add(this.lblTicks);
            this.gbxBetOdds.Controls.Add(this.spnTicks);
            this.gbxBetOdds.Controls.Add(this.rbnChangeOdds);
            this.gbxBetOdds.Controls.Add(this.rbnGivenOdds);
            this.gbxBetOdds.Font = null;
            this.gbxBetOdds.Name = "gbxBetOdds";
            this.gbxBetOdds.TabStop = false;
            // 
            // lblTicksExplain
            // 
            this.lblTicksExplain.AccessibleDescription = null;
            this.lblTicksExplain.AccessibleName = null;
            resources.ApplyResources(this.lblTicksExplain, "lblTicksExplain");
            this.lblTicksExplain.Font = null;
            this.lblTicksExplain.Name = "lblTicksExplain";
            // 
            // lblTicks
            // 
            this.lblTicks.AccessibleDescription = null;
            this.lblTicks.AccessibleName = null;
            resources.ApplyResources(this.lblTicks, "lblTicks");
            this.lblTicks.Font = null;
            this.lblTicks.Name = "lblTicks";
            // 
            // spnTicks
            // 
            this.spnTicks.AccessibleDescription = null;
            this.spnTicks.AccessibleName = null;
            resources.ApplyResources(this.spnTicks, "spnTicks");
            this.spnTicks.Font = null;
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
            1,
            0,
            0,
            0});
            // 
            // rbnChangeOdds
            // 
            this.rbnChangeOdds.AccessibleDescription = null;
            this.rbnChangeOdds.AccessibleName = null;
            resources.ApplyResources(this.rbnChangeOdds, "rbnChangeOdds");
            this.rbnChangeOdds.BackgroundImage = null;
            this.rbnChangeOdds.Font = null;
            this.rbnChangeOdds.Name = "rbnChangeOdds";
            this.rbnChangeOdds.TabStop = true;
            this.rbnChangeOdds.UseVisualStyleBackColor = true;
            // 
            // rbnGivenOdds
            // 
            this.rbnGivenOdds.AccessibleDescription = null;
            this.rbnGivenOdds.AccessibleName = null;
            resources.ApplyResources(this.rbnGivenOdds, "rbnGivenOdds");
            this.rbnGivenOdds.BackgroundImage = null;
            this.rbnGivenOdds.Font = null;
            this.rbnGivenOdds.Name = "rbnGivenOdds";
            this.rbnGivenOdds.TabStop = true;
            this.rbnGivenOdds.UseVisualStyleBackColor = true;
            // 
            // gbxBetBehaviour
            // 
            this.gbxBetBehaviour.AccessibleDescription = null;
            this.gbxBetBehaviour.AccessibleName = null;
            resources.ApplyResources(this.gbxBetBehaviour, "gbxBetBehaviour");
            this.gbxBetBehaviour.BackgroundImage = null;
            this.gbxBetBehaviour.Controls.Add(this.chkKeepInplay);
            this.gbxBetBehaviour.Controls.Add(this.chkKeepUnmatched);
            this.gbxBetBehaviour.Font = null;
            this.gbxBetBehaviour.Name = "gbxBetBehaviour";
            this.gbxBetBehaviour.TabStop = false;
            // 
            // chkKeepInplay
            // 
            this.chkKeepInplay.AccessibleDescription = null;
            this.chkKeepInplay.AccessibleName = null;
            resources.ApplyResources(this.chkKeepInplay, "chkKeepInplay");
            this.chkKeepInplay.BackgroundImage = null;
            this.chkKeepInplay.Font = null;
            this.chkKeepInplay.Name = "chkKeepInplay";
            this.chkKeepInplay.UseVisualStyleBackColor = true;
            // 
            // chkKeepUnmatched
            // 
            this.chkKeepUnmatched.AccessibleDescription = null;
            this.chkKeepUnmatched.AccessibleName = null;
            resources.ApplyResources(this.chkKeepUnmatched, "chkKeepUnmatched");
            this.chkKeepUnmatched.BackgroundImage = null;
            this.chkKeepUnmatched.Font = null;
            this.chkKeepUnmatched.Name = "chkKeepUnmatched";
            this.chkKeepUnmatched.UseVisualStyleBackColor = true;
            this.chkKeepUnmatched.CheckedChanged += new System.EventHandler(this.chkKeepUnmatched_CheckedChanged);
            // 
            // frmTradeInMoneyConfig
            // 
            this.AcceptButton = this.btnOK;
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.CancelButton = this.btnCancel;
            this.ControlBox = false;
            this.Controls.Add(this.gbxBetAmount);
            this.Controls.Add(this.gbxBetOdds);
            this.Controls.Add(this.gbxBetBehaviour);
            this.Controls.Add(this.pnlButtons);
            this.Font = null;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "frmTradeInMoneyConfig";
            ((System.ComponentModel.ISupportInitialize)(this.spnFastBetPercentValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnFastBetFixedValue)).EndInit();
            this.gbxPercentageRef.ResumeLayout(false);
            this.gbxPercentageRef.PerformLayout();
            this.gbxBetAmount.ResumeLayout(false);
            this.gbxBetAmount.PerformLayout();
            this.gbxTargetSize.ResumeLayout(false);
            this.gbxTargetSize.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spnRelativeTargetAmount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnFixedTargetAmount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnRelativeBetSize)).EndInit();
            this.pnlButtons.ResumeLayout(false);
            this.gbxBetOdds.ResumeLayout(false);
            this.gbxBetOdds.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spnTicks)).EndInit();
            this.gbxBetBehaviour.ResumeLayout(false);
            this.gbxBetBehaviour.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblPercentOf;
        private System.Windows.Forms.NumericUpDown spnFastBetPercentValue;
        private System.Windows.Forms.Label lblFastBetCurrency;
        private System.Windows.Forms.NumericUpDown spnFastBetFixedValue;
        private System.Windows.Forms.GroupBox gbxPercentageRef;
        private System.Windows.Forms.RadioButton rbnPercentageAvailibleAmount;
        private System.Windows.Forms.RadioButton rbnPercentageTotalAmount;
        private System.Windows.Forms.GroupBox gbxBetAmount;
        private System.Windows.Forms.Label lblPercentOf2;
        private System.Windows.Forms.NumericUpDown spnRelativeBetSize;
        private System.Windows.Forms.RadioButton rbnRelativeBetAmount;
        private System.Windows.Forms.RadioButton rbnPercentBetAmount;
        private System.Windows.Forms.RadioButton rbnFixedBetAmount;
        private System.Windows.Forms.Panel pnlButtons;
        private System.Windows.Forms.ComboBox cbxRelativeBetType;
        private System.Windows.Forms.Label lblOf;
        private System.Windows.Forms.ComboBox cbxRelativeTradeType;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.RadioButton rbnBetAmountByTarget;
        private System.Windows.Forms.GroupBox gbxTargetSize;
        private System.Windows.Forms.RadioButton rbnFixedTargetAmount;
        private System.Windows.Forms.Label lblCurrency2;
        private System.Windows.Forms.NumericUpDown spnFixedTargetAmount;
        private System.Windows.Forms.RadioButton rbnRelativeTargetAmount;
        private System.Windows.Forms.Label lblRelativeTargetString;
        private System.Windows.Forms.NumericUpDown spnRelativeTargetAmount;
        private System.Windows.Forms.GroupBox gbxBetOdds;
        private System.Windows.Forms.Label lblTicks;
        private System.Windows.Forms.NumericUpDown spnTicks;
        private System.Windows.Forms.RadioButton rbnChangeOdds;
        private System.Windows.Forms.RadioButton rbnGivenOdds;
        private System.Windows.Forms.Label lblTicksExplain;
        private System.Windows.Forms.GroupBox gbxBetBehaviour;
        private System.Windows.Forms.CheckBox chkKeepUnmatched;
        private System.Windows.Forms.CheckBox chkKeepInplay;
    }
}