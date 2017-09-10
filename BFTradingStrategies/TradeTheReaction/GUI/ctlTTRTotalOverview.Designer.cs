namespace net.sxtrader.bftradingstrategies.ttr.GUI
{
    partial class ctlTTRTotalOverview
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ctlTTRTotalOverview));
            this.gbxOverPL = new System.Windows.Forms.GroupBox();
            this.pnlPL = new System.Windows.Forms.Panel();
            this.gbxOverPL.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbxOverPL
            // 
            this.gbxOverPL.AccessibleDescription = null;
            this.gbxOverPL.AccessibleName = null;
            resources.ApplyResources(this.gbxOverPL, "gbxOverPL");
            this.gbxOverPL.BackgroundImage = null;
            this.gbxOverPL.Controls.Add(this.pnlPL);
            this.gbxOverPL.Font = null;
            this.gbxOverPL.Name = "gbxOverPL";
            this.gbxOverPL.TabStop = false;
            // 
            // pnlPL
            // 
            this.pnlPL.AccessibleDescription = null;
            this.pnlPL.AccessibleName = null;
            resources.ApplyResources(this.pnlPL, "pnlPL");
            this.pnlPL.BackgroundImage = null;
            this.pnlPL.Font = null;
            this.pnlPL.Name = "pnlPL";
            // 
            // ctlTTRTotalOverview
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.Controls.Add(this.gbxOverPL);
            this.Font = null;
            this.Name = "ctlTTRTotalOverview";
            this.gbxOverPL.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbxOverPL;
        private System.Windows.Forms.Panel pnlPL;
    }
}
