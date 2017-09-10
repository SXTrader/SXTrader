namespace net.sxtrader.bftradingstrategies.ttr.GUI
{
    partial class ctlFastBet
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ctlFastBet));
            this.gbxFastTrade = new System.Windows.Forms.GroupBox();
            this.btnMassLoadeLeague = new System.Windows.Forms.Button();
            this.btnStarter = new System.Windows.Forms.Button();
            this.gbxFastTrade.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbxFastTrade
            // 
            this.gbxFastTrade.Controls.Add(this.btnMassLoadeLeague);
            this.gbxFastTrade.Controls.Add(this.btnStarter);
            resources.ApplyResources(this.gbxFastTrade, "gbxFastTrade");
            this.gbxFastTrade.Name = "gbxFastTrade";
            this.gbxFastTrade.TabStop = false;
            // 
            // btnMassLoadeLeague
            // 
            resources.ApplyResources(this.btnMassLoadeLeague, "btnMassLoadeLeague");
            this.btnMassLoadeLeague.Name = "btnMassLoadeLeague";
            this.btnMassLoadeLeague.UseVisualStyleBackColor = true;
            this.btnMassLoadeLeague.Click += new System.EventHandler(this.btnMassLoadeLeague_Click);
            // 
            // btnStarter
            // 
            resources.ApplyResources(this.btnStarter, "btnStarter");
            this.btnStarter.Name = "btnStarter";
            this.btnStarter.UseVisualStyleBackColor = true;
            this.btnStarter.Click += new System.EventHandler(this.btnStarter_Click_1);
            // 
            // ctlFastBet
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gbxFastTrade);
            this.Name = "ctlFastBet";
            this.gbxFastTrade.ResumeLayout(false);
            this.gbxFastTrade.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbxFastTrade;
        private System.Windows.Forms.Button btnStarter;
        private System.Windows.Forms.Button btnMassLoadeLeague;


    }
}
