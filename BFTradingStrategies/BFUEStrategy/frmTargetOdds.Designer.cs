namespace net.sxtrader.bftradingstrategies.bfuestrategy
{
    partial class frmTargetOdds
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

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmTargetOdds));
            this.gbxBetAmount = new System.Windows.Forms.GroupBox();
            this.gbxPercentageRef = new System.Windows.Forms.GroupBox();
            this.rbnPercentageAvailibleAmount = new System.Windows.Forms.RadioButton();
            this.rbnPercentageTotalAmount = new System.Windows.Forms.RadioButton();
            this.lblPercentOf = new System.Windows.Forms.Label();
            this.spnFastBetPercentValue = new System.Windows.Forms.NumericUpDown();
            this.rbnPercentBetAmount = new System.Windows.Forms.RadioButton();
            this.lblFastBetCurrency = new System.Windows.Forms.Label();
            this.spnFastBetFixedValue = new System.Windows.Forms.NumericUpDown();
            this.rbnFixedBetAmount = new System.Windows.Forms.RadioButton();
            this.gbxOdds = new System.Windows.Forms.GroupBox();
            this.spnOdds = new System.Windows.Forms.NumericUpDown();
            this.lblOdds = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.gbxBetAmount.SuspendLayout();
            this.gbxPercentageRef.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spnFastBetPercentValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnFastBetFixedValue)).BeginInit();
            this.gbxOdds.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spnOdds)).BeginInit();
            this.SuspendLayout();
            // 
            // gbxBetAmount
            // 
            this.gbxBetAmount.Controls.Add(this.gbxPercentageRef);
            this.gbxBetAmount.Controls.Add(this.lblPercentOf);
            this.gbxBetAmount.Controls.Add(this.spnFastBetPercentValue);
            this.gbxBetAmount.Controls.Add(this.rbnPercentBetAmount);
            this.gbxBetAmount.Controls.Add(this.lblFastBetCurrency);
            this.gbxBetAmount.Controls.Add(this.spnFastBetFixedValue);
            this.gbxBetAmount.Controls.Add(this.rbnFixedBetAmount);
            resources.ApplyResources(this.gbxBetAmount, "gbxBetAmount");
            this.gbxBetAmount.Name = "gbxBetAmount";
            this.gbxBetAmount.TabStop = false;
            // 
            // gbxPercentageRef
            // 
            this.gbxPercentageRef.Controls.Add(this.rbnPercentageAvailibleAmount);
            this.gbxPercentageRef.Controls.Add(this.rbnPercentageTotalAmount);
            resources.ApplyResources(this.gbxPercentageRef, "gbxPercentageRef");
            this.gbxPercentageRef.Name = "gbxPercentageRef";
            this.gbxPercentageRef.TabStop = false;
            // 
            // rbnPercentageAvailibleAmount
            // 
            resources.ApplyResources(this.rbnPercentageAvailibleAmount, "rbnPercentageAvailibleAmount");
            this.rbnPercentageAvailibleAmount.Name = "rbnPercentageAvailibleAmount";
            this.rbnPercentageAvailibleAmount.TabStop = true;
            this.rbnPercentageAvailibleAmount.UseVisualStyleBackColor = true;
            // 
            // rbnPercentageTotalAmount
            // 
            resources.ApplyResources(this.rbnPercentageTotalAmount, "rbnPercentageTotalAmount");
            this.rbnPercentageTotalAmount.Name = "rbnPercentageTotalAmount";
            this.rbnPercentageTotalAmount.TabStop = true;
            this.rbnPercentageTotalAmount.UseVisualStyleBackColor = true;
            // 
            // lblPercentOf
            // 
            resources.ApplyResources(this.lblPercentOf, "lblPercentOf");
            this.lblPercentOf.Name = "lblPercentOf";
            // 
            // spnFastBetPercentValue
            // 
            resources.ApplyResources(this.spnFastBetPercentValue, "spnFastBetPercentValue");
            this.spnFastBetPercentValue.Name = "spnFastBetPercentValue";
            // 
            // rbnPercentBetAmount
            // 
            resources.ApplyResources(this.rbnPercentBetAmount, "rbnPercentBetAmount");
            this.rbnPercentBetAmount.Name = "rbnPercentBetAmount";
            this.rbnPercentBetAmount.TabStop = true;
            this.rbnPercentBetAmount.UseVisualStyleBackColor = true;
            // 
            // lblFastBetCurrency
            // 
            resources.ApplyResources(this.lblFastBetCurrency, "lblFastBetCurrency");
            this.lblFastBetCurrency.Name = "lblFastBetCurrency";
            // 
            // spnFastBetFixedValue
            // 
            this.spnFastBetFixedValue.DecimalPlaces = 2;
            resources.ApplyResources(this.spnFastBetFixedValue, "spnFastBetFixedValue");
            this.spnFastBetFixedValue.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.spnFastBetFixedValue.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.spnFastBetFixedValue.Name = "spnFastBetFixedValue";
            this.spnFastBetFixedValue.Value = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            // 
            // rbnFixedBetAmount
            // 
            resources.ApplyResources(this.rbnFixedBetAmount, "rbnFixedBetAmount");
            this.rbnFixedBetAmount.Name = "rbnFixedBetAmount";
            this.rbnFixedBetAmount.TabStop = true;
            this.rbnFixedBetAmount.UseVisualStyleBackColor = true;
            // 
            // gbxOdds
            // 
            this.gbxOdds.Controls.Add(this.spnOdds);
            this.gbxOdds.Controls.Add(this.lblOdds);
            resources.ApplyResources(this.gbxOdds, "gbxOdds");
            this.gbxOdds.Name = "gbxOdds";
            this.gbxOdds.TabStop = false;
            // 
            // spnOdds
            // 
            this.spnOdds.DecimalPlaces = 2;
            this.spnOdds.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            resources.ApplyResources(this.spnOdds, "spnOdds");
            this.spnOdds.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.spnOdds.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.spnOdds.Name = "spnOdds";
            this.spnOdds.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.spnOdds.ValueChanged += new System.EventHandler(this.spnOdds_ValueChanged);
            // 
            // lblOdds
            // 
            resources.ApplyResources(this.lblOdds, "lblOdds");
            this.lblOdds.Name = "lblOdds";
            // 
            // btnOK
            // 
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.Name = "btnOK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // frmTargetOdds
            // 
            this.AcceptButton = this.btnOK;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ControlBox = false;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.gbxOdds);
            this.Controls.Add(this.gbxBetAmount);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "frmTargetOdds";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.gbxBetAmount.ResumeLayout(false);
            this.gbxBetAmount.PerformLayout();
            this.gbxPercentageRef.ResumeLayout(false);
            this.gbxPercentageRef.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spnFastBetPercentValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnFastBetFixedValue)).EndInit();
            this.gbxOdds.ResumeLayout(false);
            this.gbxOdds.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spnOdds)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbxBetAmount;
        private System.Windows.Forms.GroupBox gbxPercentageRef;
        private System.Windows.Forms.RadioButton rbnPercentageAvailibleAmount;
        private System.Windows.Forms.RadioButton rbnPercentageTotalAmount;
        private System.Windows.Forms.Label lblPercentOf;
        private System.Windows.Forms.NumericUpDown spnFastBetPercentValue;
        private System.Windows.Forms.RadioButton rbnPercentBetAmount;
        private System.Windows.Forms.Label lblFastBetCurrency;
        private System.Windows.Forms.NumericUpDown spnFastBetFixedValue;
        private System.Windows.Forms.RadioButton rbnFixedBetAmount;
        private System.Windows.Forms.GroupBox gbxOdds;
        private System.Windows.Forms.NumericUpDown spnOdds;
        private System.Windows.Forms.Label lblOdds;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;

    }
}