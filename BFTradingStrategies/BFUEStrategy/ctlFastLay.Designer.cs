namespace net.sxtrader.bftradingstrategies.bfuestrategy
{
    partial class ctlFastLay
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ctlFastLay));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnIPTrader = new System.Windows.Forms.Button();
            this.btnTargetOdds = new System.Windows.Forms.Button();
            this.btnFastBet = new System.Windows.Forms.Button();
            this.gbxExistingTrades = new System.Windows.Forms.GroupBox();
            this.lblLayNumber = new System.Windows.Forms.Label();
            this.lblLay = new System.Windows.Forms.Label();
            this.lblSlash = new System.Windows.Forms.Label();
            this.lblBackNumber = new System.Windows.Forms.Label();
            this.lblBack = new System.Windows.Forms.Label();
            this.lblMarketVolumeNumber = new System.Windows.Forms.Label();
            this.lblMarketVolume = new System.Windows.Forms.Label();
            this.cmsFastLay = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.fastLayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.delayedLayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1.SuspendLayout();
            this.gbxExistingTrades.SuspendLayout();
            this.cmsFastLay.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnIPTrader);
            this.groupBox1.Controls.Add(this.btnTargetOdds);
            this.groupBox1.Controls.Add(this.btnFastBet);
            this.groupBox1.Controls.Add(this.gbxExistingTrades);
            this.groupBox1.Controls.Add(this.lblMarketVolumeNumber);
            this.groupBox1.Controls.Add(this.lblMarketVolume);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // btnIPTrader
            // 
            this.btnIPTrader.BackColor = System.Drawing.Color.Pink;
            resources.ApplyResources(this.btnIPTrader, "btnIPTrader");
            this.btnIPTrader.Name = "btnIPTrader";
            this.btnIPTrader.UseVisualStyleBackColor = false;
            this.btnIPTrader.Click += new System.EventHandler(this.btnIPTrader_Click);
            // 
            // btnTargetOdds
            // 
            resources.ApplyResources(this.btnTargetOdds, "btnTargetOdds");
            this.btnTargetOdds.BackColor = System.Drawing.Color.Pink;
            this.btnTargetOdds.Name = "btnTargetOdds";
            this.btnTargetOdds.UseVisualStyleBackColor = false;
            this.btnTargetOdds.Click += new System.EventHandler(this.btnTargetOdds_Click);
            // 
            // btnFastBet
            // 
            this.btnFastBet.BackColor = System.Drawing.Color.Pink;
            resources.ApplyResources(this.btnFastBet, "btnFastBet");
            this.btnFastBet.Name = "btnFastBet";
            this.btnFastBet.UseVisualStyleBackColor = false;
            this.btnFastBet.Click += new System.EventHandler(this.btnFastBet_Click);
            // 
            // gbxExistingTrades
            // 
            this.gbxExistingTrades.Controls.Add(this.lblLayNumber);
            this.gbxExistingTrades.Controls.Add(this.lblLay);
            this.gbxExistingTrades.Controls.Add(this.lblSlash);
            this.gbxExistingTrades.Controls.Add(this.lblBackNumber);
            this.gbxExistingTrades.Controls.Add(this.lblBack);
            resources.ApplyResources(this.gbxExistingTrades, "gbxExistingTrades");
            this.gbxExistingTrades.Name = "gbxExistingTrades";
            this.gbxExistingTrades.TabStop = false;
            // 
            // lblLayNumber
            // 
            resources.ApplyResources(this.lblLayNumber, "lblLayNumber");
            this.lblLayNumber.Name = "lblLayNumber";
            // 
            // lblLay
            // 
            resources.ApplyResources(this.lblLay, "lblLay");
            this.lblLay.Name = "lblLay";
            // 
            // lblSlash
            // 
            resources.ApplyResources(this.lblSlash, "lblSlash");
            this.lblSlash.Name = "lblSlash";
            // 
            // lblBackNumber
            // 
            resources.ApplyResources(this.lblBackNumber, "lblBackNumber");
            this.lblBackNumber.Name = "lblBackNumber";
            this.lblBackNumber.Resize += new System.EventHandler(this.lblBackNumber_Resize);
            // 
            // lblBack
            // 
            resources.ApplyResources(this.lblBack, "lblBack");
            this.lblBack.Name = "lblBack";
            // 
            // lblMarketVolumeNumber
            // 
            resources.ApplyResources(this.lblMarketVolumeNumber, "lblMarketVolumeNumber");
            this.lblMarketVolumeNumber.Name = "lblMarketVolumeNumber";
            // 
            // lblMarketVolume
            // 
            resources.ApplyResources(this.lblMarketVolume, "lblMarketVolume");
            this.lblMarketVolume.Name = "lblMarketVolume";
            // 
            // cmsFastLay
            // 
            this.cmsFastLay.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fastLayToolStripMenuItem,
            this.delayedLayToolStripMenuItem});
            this.cmsFastLay.Name = "cmsFastLay";
            resources.ApplyResources(this.cmsFastLay, "cmsFastLay");
            // 
            // fastLayToolStripMenuItem
            // 
            this.fastLayToolStripMenuItem.Name = "fastLayToolStripMenuItem";
            resources.ApplyResources(this.fastLayToolStripMenuItem, "fastLayToolStripMenuItem");
            // 
            // delayedLayToolStripMenuItem
            // 
            this.delayedLayToolStripMenuItem.Name = "delayedLayToolStripMenuItem";
            resources.ApplyResources(this.delayedLayToolStripMenuItem, "delayedLayToolStripMenuItem");
            // 
            // ctlFastLay
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "ctlFastLay";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.gbxExistingTrades.ResumeLayout(false);
            this.gbxExistingTrades.PerformLayout();
            this.cmsFastLay.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblMarketVolumeNumber;
        private System.Windows.Forms.Label lblMarketVolume;
        private System.Windows.Forms.GroupBox gbxExistingTrades;
        private System.Windows.Forms.Label lblBack;
        private System.Windows.Forms.Label lblBackNumber;
        private System.Windows.Forms.Label lblLayNumber;
        private System.Windows.Forms.Label lblLay;
        private System.Windows.Forms.Label lblSlash;
        private System.Windows.Forms.Button btnFastBet;
        //private SplitButtonDemo.SplitButton btnDrpDwnFastBet;
        private System.Windows.Forms.ContextMenuStrip cmsFastLay;
        private System.Windows.Forms.ToolStripMenuItem fastLayToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem delayedLayToolStripMenuItem;
        private System.Windows.Forms.Button btnTargetOdds;
        private System.Windows.Forms.Button btnIPTrader;
    }
}
