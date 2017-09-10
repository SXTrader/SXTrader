namespace net.sxtrader.bftradingstrategies.bfuestrategy.IPTraderObj
{
    partial class frmIPTradeConfig
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmIPTradeConfig));
            this.pnlButtons = new System.Windows.Forms.Panel();
            this.btnOpenTemplate = new System.Windows.Forms.Button();
            this.btnSaveTemplate = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.pnlIPDescs2 = new System.Windows.Forms.Panel();
            this.sfdTemplate = new System.Windows.Forms.SaveFileDialog();
            this.ofdTemplate = new System.Windows.Forms.OpenFileDialog();
            this.ctlIPTElementEdit1 = new net.sxtrader.bftradingstrategies.bfuestrategy.IPTraderObj.ctlIPTElementEdit();
            this.pnlButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlButtons
            // 
            this.pnlButtons.AccessibleDescription = null;
            this.pnlButtons.AccessibleName = null;
            resources.ApplyResources(this.pnlButtons, "pnlButtons");
            this.pnlButtons.BackgroundImage = null;
            this.pnlButtons.Controls.Add(this.btnOpenTemplate);
            this.pnlButtons.Controls.Add(this.btnSaveTemplate);
            this.pnlButtons.Controls.Add(this.btnOK);
            this.pnlButtons.Font = null;
            this.pnlButtons.Name = "pnlButtons";
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
            // btnOK
            // 
            this.btnOK.AccessibleDescription = null;
            this.btnOK.AccessibleName = null;
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.BackgroundImage = null;
            this.btnOK.Font = null;
            this.btnOK.Name = "btnOK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // pnlIPDescs2
            // 
            this.pnlIPDescs2.AccessibleDescription = null;
            this.pnlIPDescs2.AccessibleName = null;
            resources.ApplyResources(this.pnlIPDescs2, "pnlIPDescs2");
            this.pnlIPDescs2.BackgroundImage = null;
            this.pnlIPDescs2.Font = null;
            this.pnlIPDescs2.Name = "pnlIPDescs2";
            // 
            // sfdTemplate
            // 
            resources.ApplyResources(this.sfdTemplate, "sfdTemplate");
            // 
            // ofdTemplate
            // 
            resources.ApplyResources(this.ofdTemplate, "ofdTemplate");
            // 
            // ctlIPTElementEdit1
            // 
            this.ctlIPTElementEdit1.AccessibleDescription = null;
            this.ctlIPTElementEdit1.AccessibleName = null;
            resources.ApplyResources(this.ctlIPTElementEdit1, "ctlIPTElementEdit1");
            this.ctlIPTElementEdit1.BackgroundImage = null;
            this.ctlIPTElementEdit1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ctlIPTElementEdit1.Font = null;
            this.ctlIPTElementEdit1.IPTConfigElement = null;
            this.ctlIPTElementEdit1.Name = "ctlIPTElementEdit1";
            this.ctlIPTElementEdit1.SaveIPTConfigElement += new System.EventHandler<net.sxtrader.bftradingstrategies.bfuestrategy.IPTraderObj.BFUEIPTElementSaveEventArgs>(this.ctlIPTElementEdit1_SaveIPTConfigElement);
            // 
            // frmIPTradeConfig
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.ControlBox = false;
            this.Controls.Add(this.pnlIPDescs2);
            this.Controls.Add(this.ctlIPTElementEdit1);
            this.Controls.Add(this.pnlButtons);
            this.Font = null;
            this.Icon = null;
            this.Name = "frmIPTradeConfig";
            this.pnlButtons.ResumeLayout(false);
            this.pnlButtons.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlButtons;
        private System.Windows.Forms.Button btnOK;
        private ctlIPTElementEdit ctlIPTElementEdit1;
        private System.Windows.Forms.Panel pnlIPDescs2;
        private System.Windows.Forms.Button btnSaveTemplate;
        private System.Windows.Forms.SaveFileDialog sfdTemplate;
        private System.Windows.Forms.Button btnOpenTemplate;
        private System.Windows.Forms.OpenFileDialog ofdTemplate;
    }
}