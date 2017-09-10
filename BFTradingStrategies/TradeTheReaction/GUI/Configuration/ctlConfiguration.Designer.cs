namespace net.sxtrader.bftradingstrategies.ttr.GUI.Configuration
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
            this.tbpSL00 = new System.Windows.Forms.TabPage();
            this.tbpOverUnder = new System.Windows.Forms.TabPage();
            this.tbpCorrectScore = new System.Windows.Forms.TabPage();
            this.tbpMoney = new System.Windows.Forms.TabPage();
            this.tbcConfig.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbcConfig
            // 
            this.tbcConfig.AccessibleDescription = null;
            this.tbcConfig.AccessibleName = null;
            resources.ApplyResources(this.tbcConfig, "tbcConfig");
            this.tbcConfig.BackgroundImage = null;
            this.tbcConfig.Controls.Add(this.tbpGeneral);
            this.tbcConfig.Controls.Add(this.tbpSL00);
            this.tbcConfig.Controls.Add(this.tbpOverUnder);
            this.tbcConfig.Controls.Add(this.tbpCorrectScore);
            this.tbcConfig.Controls.Add(this.tbpMoney);
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
            this.tbpGeneral.Font = null;
            this.tbpGeneral.Name = "tbpGeneral";
            this.tbpGeneral.UseVisualStyleBackColor = true;
            // 
            // tbpSL00
            // 
            this.tbpSL00.AccessibleDescription = null;
            this.tbpSL00.AccessibleName = null;
            resources.ApplyResources(this.tbpSL00, "tbpSL00");
            this.tbpSL00.BackgroundImage = null;
            this.tbpSL00.Font = null;
            this.tbpSL00.Name = "tbpSL00";
            this.tbpSL00.UseVisualStyleBackColor = true;
            // 
            // tbpOverUnder
            // 
            this.tbpOverUnder.AccessibleDescription = null;
            this.tbpOverUnder.AccessibleName = null;
            resources.ApplyResources(this.tbpOverUnder, "tbpOverUnder");
            this.tbpOverUnder.BackgroundImage = null;
            this.tbpOverUnder.Font = null;
            this.tbpOverUnder.Name = "tbpOverUnder";
            this.tbpOverUnder.UseVisualStyleBackColor = true;
            // 
            // tbpCorrectScore
            // 
            this.tbpCorrectScore.AccessibleDescription = null;
            this.tbpCorrectScore.AccessibleName = null;
            resources.ApplyResources(this.tbpCorrectScore, "tbpCorrectScore");
            this.tbpCorrectScore.BackgroundImage = null;
            this.tbpCorrectScore.Font = null;
            this.tbpCorrectScore.Name = "tbpCorrectScore";
            this.tbpCorrectScore.UseVisualStyleBackColor = true;
            // 
            // tbpMoney
            // 
            this.tbpMoney.AccessibleDescription = null;
            this.tbpMoney.AccessibleName = null;
            resources.ApplyResources(this.tbpMoney, "tbpMoney");
            this.tbpMoney.BackgroundImage = null;
            this.tbpMoney.Font = null;
            this.tbpMoney.Name = "tbpMoney";
            this.tbpMoney.UseVisualStyleBackColor = true;
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
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tbcConfig;
        private System.Windows.Forms.TabPage tbpSL00;
        private System.Windows.Forms.TabPage tbpMoney;
        private System.Windows.Forms.TabPage tbpGeneral;
        private System.Windows.Forms.TabPage tbpOverUnder;
        private System.Windows.Forms.TabPage tbpCorrectScore;
        //private ctlSL00Configuration ctlSL00Cfg;
    }
}
