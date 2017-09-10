namespace net.sxtrader.bftradingstrategies.ttr.GUI.Configuration
{
    partial class ctlGenConfiguration
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ctlGenConfiguration));
            this.lblStartMinutes = new System.Windows.Forms.Label();
            this.spnStartMinutes = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.spnStartMinutes)).BeginInit();
            this.SuspendLayout();
            // 
            // lblStartMinutes
            // 
            this.lblStartMinutes.AccessibleDescription = null;
            this.lblStartMinutes.AccessibleName = null;
            resources.ApplyResources(this.lblStartMinutes, "lblStartMinutes");
            this.lblStartMinutes.Font = null;
            this.lblStartMinutes.Name = "lblStartMinutes";
            // 
            // spnStartMinutes
            // 
            this.spnStartMinutes.AccessibleDescription = null;
            this.spnStartMinutes.AccessibleName = null;
            resources.ApplyResources(this.spnStartMinutes, "spnStartMinutes");
            this.spnStartMinutes.Font = null;
            this.spnStartMinutes.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.spnStartMinutes.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.spnStartMinutes.Name = "spnStartMinutes";
            this.spnStartMinutes.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // ctlGenConfiguration
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.Controls.Add(this.spnStartMinutes);
            this.Controls.Add(this.lblStartMinutes);
            this.Font = null;
            this.Name = "ctlGenConfiguration";
            ((System.ComponentModel.ISupportInitialize)(this.spnStartMinutes)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblStartMinutes;
        private System.Windows.Forms.NumericUpDown spnStartMinutes;
    }
}
