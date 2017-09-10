namespace net.sxtrader.bftradingstrategies.ttr.GUI
{
    partial class ctlAutoTradeStarter
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ctlAutoTradeStarter));
            this.pnlButtons = new System.Windows.Forms.Panel();
            this.btnOpenTemplate = new System.Windows.Forms.Button();
            this.btnSaveTemplate = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.pnlConfigEdit = new System.Windows.Forms.Panel();
            this.ctlConfigElement = new net.sxtrader.bftradingstrategies.ttr.TradeStarter.ctlASConfigElement();
            this.pnlConfigDisplay = new System.Windows.Forms.Panel();
            this.sfdTemplate = new System.Windows.Forms.SaveFileDialog();
            this.ofdTemplate = new System.Windows.Forms.OpenFileDialog();
            this.pnlButtons.SuspendLayout();
            this.pnlConfigEdit.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlButtons
            // 
            this.pnlButtons.AccessibleDescription = null;
            this.pnlButtons.AccessibleName = null;
            resources.ApplyResources(this.pnlButtons, "pnlButtons");
            this.pnlButtons.BackgroundImage = null;
            this.pnlButtons.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pnlButtons.Controls.Add(this.btnOpenTemplate);
            this.pnlButtons.Controls.Add(this.btnSaveTemplate);
            this.pnlButtons.Controls.Add(this.btnClose);
            this.pnlButtons.Font = null;
            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.SizeChanged += new System.EventHandler(this.pnlButtons_SizeChanged);
            // 
            // btnOpenTemplate
            // 
            this.btnOpenTemplate.AccessibleDescription = null;
            this.btnOpenTemplate.AccessibleName = null;
            resources.ApplyResources(this.btnOpenTemplate, "btnOpenTemplate");
            this.btnOpenTemplate.BackgroundImage = null;
            this.btnOpenTemplate.Font = null;
            this.btnOpenTemplate.Name = "btnOpenTemplate";
            this.btnOpenTemplate.UseVisualStyleBackColor = true;
            this.btnOpenTemplate.Click += new System.EventHandler(this.btnOpenTemplate_Click);
            // 
            // btnSaveTemplate
            // 
            this.btnSaveTemplate.AccessibleDescription = null;
            this.btnSaveTemplate.AccessibleName = null;
            resources.ApplyResources(this.btnSaveTemplate, "btnSaveTemplate");
            this.btnSaveTemplate.BackgroundImage = null;
            this.btnSaveTemplate.Font = null;
            this.btnSaveTemplate.Name = "btnSaveTemplate";
            this.btnSaveTemplate.UseVisualStyleBackColor = true;
            this.btnSaveTemplate.Click += new System.EventHandler(this.btnSaveTemplate_Click);
            // 
            // btnClose
            // 
            this.btnClose.AccessibleDescription = null;
            this.btnClose.AccessibleName = null;
            resources.ApplyResources(this.btnClose, "btnClose");
            this.btnClose.BackgroundImage = null;
            this.btnClose.Font = null;
            this.btnClose.Name = "btnClose";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // pnlConfigEdit
            // 
            this.pnlConfigEdit.AccessibleDescription = null;
            this.pnlConfigEdit.AccessibleName = null;
            resources.ApplyResources(this.pnlConfigEdit, "pnlConfigEdit");
            this.pnlConfigEdit.BackgroundImage = null;
            this.pnlConfigEdit.Controls.Add(this.ctlConfigElement);
            this.pnlConfigEdit.Font = null;
            this.pnlConfigEdit.Name = "pnlConfigEdit";
            // 
            // ctlConfigElement
            // 
            this.ctlConfigElement.AccessibleDescription = null;
            this.ctlConfigElement.AccessibleName = null;
            resources.ApplyResources(this.ctlConfigElement, "ctlConfigElement");
            this.ctlConfigElement.BackgroundImage = null;
            this.ctlConfigElement.Font = null;
            this.ctlConfigElement.Name = "ctlConfigElement";
            this.ctlConfigElement.TSConfigElement = null;
            this.ctlConfigElement.SaveASConfigElement += new System.EventHandler<net.sxtrader.bftradingstrategies.ttr.TradeStarter.TTRASElementSaveEventArgs>(this.ctlConfigElement_SaveASConfigElement);
            // 
            // pnlConfigDisplay
            // 
            this.pnlConfigDisplay.AccessibleDescription = null;
            this.pnlConfigDisplay.AccessibleName = null;
            resources.ApplyResources(this.pnlConfigDisplay, "pnlConfigDisplay");
            this.pnlConfigDisplay.BackgroundImage = null;
            this.pnlConfigDisplay.Font = null;
            this.pnlConfigDisplay.Name = "pnlConfigDisplay";
            // 
            // sfdTemplate
            // 
            resources.ApplyResources(this.sfdTemplate, "sfdTemplate");
            // 
            // ofdTemplate
            // 
            resources.ApplyResources(this.ofdTemplate, "ofdTemplate");
            // 
            // ctlAutoTradeStarter
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.Controls.Add(this.pnlConfigDisplay);
            this.Controls.Add(this.pnlConfigEdit);
            this.Controls.Add(this.pnlButtons);
            this.Font = null;
            this.Name = "ctlAutoTradeStarter";
            this.SizeChanged += new System.EventHandler(this.ctlAutoTradeStarter_SizeChanged);
            this.pnlButtons.ResumeLayout(false);
            this.pnlButtons.PerformLayout();
            this.pnlConfigEdit.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlButtons;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Panel pnlConfigEdit;
        private System.Windows.Forms.Panel pnlConfigDisplay;
        private net.sxtrader.bftradingstrategies.ttr.TradeStarter.ctlASConfigElement ctlConfigElement;
        private System.Windows.Forms.Button btnOpenTemplate;
        private System.Windows.Forms.Button btnSaveTemplate;
        private System.Windows.Forms.SaveFileDialog sfdTemplate;
        private System.Windows.Forms.OpenFileDialog ofdTemplate;
    }
}
