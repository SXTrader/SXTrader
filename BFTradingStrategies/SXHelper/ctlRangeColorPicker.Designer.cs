namespace net.sxtrader.bftradingstrategies.sxhelper
{
    partial class ctlRangeColorPicker
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ctlRangeColorPicker));
            this.lblBetween = new System.Windows.Forms.Label();
            this.lblAnd = new System.Windows.Forms.Label();
            this.spnHi = new System.Windows.Forms.NumericUpDown();
            this.cbtColor = new net.sxtrader.muk.ColorButton();
            this.btnDelete = new System.Windows.Forms.Button();
            this.spnLo = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.spnHi)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnLo)).BeginInit();
            this.SuspendLayout();
            // 
            // lblBetween
            // 
            resources.ApplyResources(this.lblBetween, "lblBetween");
            this.lblBetween.Name = "lblBetween";
            // 
            // lblAnd
            // 
            resources.ApplyResources(this.lblAnd, "lblAnd");
            this.lblAnd.Name = "lblAnd";
            // 
            // spnHi
            // 
            this.spnHi.DecimalPlaces = 2;
            resources.ApplyResources(this.spnHi, "spnHi");
            this.spnHi.Name = "spnHi";
            this.spnHi.ValueChanged += new System.EventHandler(this.spnHi_ValueChanged);
            // 
            // cbtColor
            // 
            this.cbtColor.Automatic = "Automatic";
            this.cbtColor.Color = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.cbtColor, "cbtColor");
            this.cbtColor.MoreColors = "More Colors...";
            this.cbtColor.Name = "cbtColor";
            this.cbtColor.Changed += new System.EventHandler(this.cbtColor_Changed);
            // 
            // btnDelete
            // 
            resources.ApplyResources(this.btnDelete, "btnDelete");
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // spnLo
            // 
            this.spnLo.DecimalPlaces = 2;
            resources.ApplyResources(this.spnLo, "spnLo");
            this.spnLo.Name = "spnLo";
            this.spnLo.ValueChanged += new System.EventHandler(this.spnLo_ValueChanged);
            // 
            // ctlRangeColorPicker
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.spnLo);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.cbtColor);
            this.Controls.Add(this.spnHi);
            this.Controls.Add(this.lblAnd);
            this.Controls.Add(this.lblBetween);
            this.Name = "ctlRangeColorPicker";
            ((System.ComponentModel.ISupportInitialize)(this.spnHi)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnLo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblBetween;
        private System.Windows.Forms.Label lblAnd;
        private System.Windows.Forms.NumericUpDown spnHi;
        private net.sxtrader.muk.ColorButton cbtColor;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.NumericUpDown spnLo;
    }
}
