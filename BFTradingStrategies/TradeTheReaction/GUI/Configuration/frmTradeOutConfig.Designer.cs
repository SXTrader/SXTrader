namespace net.sxtrader.bftradingstrategies.ttr.GUI.Configuration
{
    partial class frmTradeOutConfig
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmTradeOutConfig));
            this.pnlButtons = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.pnlConfigRule = new System.Windows.Forms.Panel();
            this.pnlDisplayRules = new System.Windows.Forms.Panel();
            this.pnlButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlButtons
            // 
            this.pnlButtons.AccessibleDescription = null;
            this.pnlButtons.AccessibleName = null;
            resources.ApplyResources(this.pnlButtons, "pnlButtons");
            this.pnlButtons.BackgroundImage = null;
            this.pnlButtons.Controls.Add(this.btnCancel);
            this.pnlButtons.Controls.Add(this.btnOK);
            this.pnlButtons.Font = null;
            this.pnlButtons.Name = "pnlButtons";
            // 
            // btnCancel
            // 
            this.btnCancel.AccessibleDescription = null;
            this.btnCancel.AccessibleName = null;
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.BackgroundImage = null;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Font = null;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.AccessibleDescription = null;
            this.btnOK.AccessibleName = null;
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.BackgroundImage = null;
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Font = null;
            this.btnOK.Name = "btnOK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // pnlConfigRule
            // 
            this.pnlConfigRule.AccessibleDescription = null;
            this.pnlConfigRule.AccessibleName = null;
            resources.ApplyResources(this.pnlConfigRule, "pnlConfigRule");
            this.pnlConfigRule.BackgroundImage = null;
            this.pnlConfigRule.Font = null;
            this.pnlConfigRule.Name = "pnlConfigRule";
            // 
            // pnlDisplayRules
            // 
            this.pnlDisplayRules.AccessibleDescription = null;
            this.pnlDisplayRules.AccessibleName = null;
            resources.ApplyResources(this.pnlDisplayRules, "pnlDisplayRules");
            this.pnlDisplayRules.BackgroundImage = null;
            this.pnlDisplayRules.Font = null;
            this.pnlDisplayRules.Name = "pnlDisplayRules";
            // 
            // frmTradeOutConfig
            // 
            this.AcceptButton = this.btnCancel;
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.CancelButton = this.btnOK;
            this.ControlBox = false;
            this.Controls.Add(this.pnlDisplayRules);
            this.Controls.Add(this.pnlConfigRule);
            this.Controls.Add(this.pnlButtons);
            this.Font = null;
            this.Name = "frmTradeOutConfig";
            this.pnlButtons.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlButtons;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Panel pnlConfigRule;
        private System.Windows.Forms.Panel pnlDisplayRules;
    }
}