namespace net.sxtrader.bftradingstrategies.sxstatisticbase.GUI
{
    partial class ctlStatisticRangePicker
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ctlStatisticRangePicker));
            this.cbxTeam = new System.Windows.Forms.ComboBox();
            this.cbxHomeAway = new System.Windows.Forms.ComboBox();
            this.cbxStatistic = new System.Windows.Forms.ComboBox();
            this.spnLo = new System.Windows.Forms.NumericUpDown();
            this.spnHi = new System.Windows.Forms.NumericUpDown();
            this.lblAnd = new System.Windows.Forms.Label();
            this.lblBetween = new System.Windows.Forms.Label();
            this.btnDelete = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.spnLo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnHi)).BeginInit();
            this.SuspendLayout();
            // 
            // cbxTeam
            // 
            this.cbxTeam.AccessibleDescription = null;
            this.cbxTeam.AccessibleName = null;
            resources.ApplyResources(this.cbxTeam, "cbxTeam");
            this.cbxTeam.BackgroundImage = null;
            this.cbxTeam.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxTeam.Font = null;
            this.cbxTeam.FormattingEnabled = true;
            this.cbxTeam.Name = "cbxTeam";
            this.cbxTeam.SelectedIndexChanged += new System.EventHandler(this.cbxTeam_SelectedIndexChanged);
            this.cbxTeam.SelectedValueChanged += new System.EventHandler(this.cbxTeam_SelectedValueChanged);
            // 
            // cbxHomeAway
            // 
            this.cbxHomeAway.AccessibleDescription = null;
            this.cbxHomeAway.AccessibleName = null;
            resources.ApplyResources(this.cbxHomeAway, "cbxHomeAway");
            this.cbxHomeAway.BackgroundImage = null;
            this.cbxHomeAway.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxHomeAway.Font = null;
            this.cbxHomeAway.FormattingEnabled = true;
            this.cbxHomeAway.Name = "cbxHomeAway";
            this.cbxHomeAway.SelectedIndexChanged += new System.EventHandler(this.cbxHomeAway_SelectedIndexChanged);
            // 
            // cbxStatistic
            // 
            this.cbxStatistic.AccessibleDescription = null;
            this.cbxStatistic.AccessibleName = null;
            resources.ApplyResources(this.cbxStatistic, "cbxStatistic");
            this.cbxStatistic.BackgroundImage = null;
            this.cbxStatistic.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxStatistic.Font = null;
            this.cbxStatistic.FormattingEnabled = true;
            this.cbxStatistic.Name = "cbxStatistic";
            this.cbxStatistic.SelectedIndexChanged += new System.EventHandler(this.cbxStatistic_SelectedIndexChanged);
            // 
            // spnLo
            // 
            this.spnLo.AccessibleDescription = null;
            this.spnLo.AccessibleName = null;
            resources.ApplyResources(this.spnLo, "spnLo");
            this.spnLo.DecimalPlaces = 2;
            this.spnLo.Font = null;
            this.spnLo.Name = "spnLo";
            this.spnLo.ValueChanged += new System.EventHandler(this.spnLo_ValueChanged);
            // 
            // spnHi
            // 
            this.spnHi.AccessibleDescription = null;
            this.spnHi.AccessibleName = null;
            resources.ApplyResources(this.spnHi, "spnHi");
            this.spnHi.DecimalPlaces = 2;
            this.spnHi.Font = null;
            this.spnHi.Name = "spnHi";
            this.spnHi.ValueChanged += new System.EventHandler(this.spnHi_ValueChanged);
            // 
            // lblAnd
            // 
            this.lblAnd.AccessibleDescription = null;
            this.lblAnd.AccessibleName = null;
            resources.ApplyResources(this.lblAnd, "lblAnd");
            this.lblAnd.Font = null;
            this.lblAnd.Name = "lblAnd";
            // 
            // lblBetween
            // 
            this.lblBetween.AccessibleDescription = null;
            this.lblBetween.AccessibleName = null;
            resources.ApplyResources(this.lblBetween, "lblBetween");
            this.lblBetween.Font = null;
            this.lblBetween.Name = "lblBetween";
            // 
            // btnDelete
            // 
            this.btnDelete.AccessibleDescription = null;
            this.btnDelete.AccessibleName = null;
            resources.ApplyResources(this.btnDelete, "btnDelete");
            this.btnDelete.BackgroundImage = null;
            this.btnDelete.Font = null;
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // ctlStatisticRangePicker
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.spnLo);
            this.Controls.Add(this.spnHi);
            this.Controls.Add(this.lblAnd);
            this.Controls.Add(this.lblBetween);
            this.Controls.Add(this.cbxStatistic);
            this.Controls.Add(this.cbxHomeAway);
            this.Controls.Add(this.cbxTeam);
            this.Font = null;
            this.Name = "ctlStatisticRangePicker";
            ((System.ComponentModel.ISupportInitialize)(this.spnLo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnHi)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbxTeam;
        private System.Windows.Forms.ComboBox cbxHomeAway;
        private System.Windows.Forms.ComboBox cbxStatistic;
        private System.Windows.Forms.NumericUpDown spnLo;
        private System.Windows.Forms.NumericUpDown spnHi;
        private System.Windows.Forms.Label lblAnd;
        private System.Windows.Forms.Label lblBetween;
        private System.Windows.Forms.Button btnDelete;
        //private System.Windows.Forms.Button btnDelete;
    }
}
