namespace net.sxtrader.bftradingstrategies.ttr.TradeStarter
{
    partial class ctlASConfigElement
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ctlASConfigElement));
            this.pnlTradeTypeSelection = new System.Windows.Forms.Panel();
            this.cbxTradeType = new System.Windows.Forms.ComboBox();
            this.lblTradeType = new System.Windows.Forms.Label();
            this.pnlButtons = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnNew = new System.Windows.Forms.Button();
            this.pnlASElementConfig = new System.Windows.Forms.Panel();
            this.pnlTradeTypeSelection.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlTradeTypeSelection
            // 
            this.pnlTradeTypeSelection.AccessibleDescription = null;
            this.pnlTradeTypeSelection.AccessibleName = null;
            resources.ApplyResources(this.pnlTradeTypeSelection, "pnlTradeTypeSelection");
            this.pnlTradeTypeSelection.BackgroundImage = null;
            this.pnlTradeTypeSelection.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pnlTradeTypeSelection.Controls.Add(this.cbxTradeType);
            this.pnlTradeTypeSelection.Controls.Add(this.lblTradeType);
            this.pnlTradeTypeSelection.Font = null;
            this.pnlTradeTypeSelection.Name = "pnlTradeTypeSelection";
            // 
            // cbxTradeType
            // 
            this.cbxTradeType.AccessibleDescription = null;
            this.cbxTradeType.AccessibleName = null;
            resources.ApplyResources(this.cbxTradeType, "cbxTradeType");
            this.cbxTradeType.BackgroundImage = null;
            this.cbxTradeType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxTradeType.Font = null;
            this.cbxTradeType.FormattingEnabled = true;
            this.cbxTradeType.Name = "cbxTradeType";
            this.cbxTradeType.SelectedIndexChanged += new System.EventHandler(this.cbxTradeType_SelectedIndexChanged);
            // 
            // lblTradeType
            // 
            this.lblTradeType.AccessibleDescription = null;
            this.lblTradeType.AccessibleName = null;
            resources.ApplyResources(this.lblTradeType, "lblTradeType");
            this.lblTradeType.Font = null;
            this.lblTradeType.Name = "lblTradeType";
            // 
            // pnlButtons
            // 
            this.pnlButtons.AccessibleDescription = null;
            this.pnlButtons.AccessibleName = null;
            resources.ApplyResources(this.pnlButtons, "pnlButtons");
            this.pnlButtons.BackgroundImage = null;
            this.pnlButtons.Controls.Add(this.btnCancel);
            this.pnlButtons.Controls.Add(this.btnSave);
            this.pnlButtons.Controls.Add(this.btnNew);
            this.pnlButtons.Font = null;
            this.pnlButtons.Name = "pnlButtons";
            // 
            // btnCancel
            // 
            this.btnCancel.AccessibleDescription = null;
            this.btnCancel.AccessibleName = null;
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.BackgroundImage = null;
            this.btnCancel.Font = null;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            this.btnSave.AccessibleDescription = null;
            this.btnSave.AccessibleName = null;
            resources.ApplyResources(this.btnSave, "btnSave");
            this.btnSave.BackgroundImage = null;
            this.btnSave.Font = null;
            this.btnSave.Name = "btnSave";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnNew
            // 
            this.btnNew.AccessibleDescription = null;
            this.btnNew.AccessibleName = null;
            resources.ApplyResources(this.btnNew, "btnNew");
            this.btnNew.BackgroundImage = null;
            this.btnNew.Font = null;
            this.btnNew.Name = "btnNew";
            this.btnNew.UseVisualStyleBackColor = true;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // pnlASElementConfig
            // 
            this.pnlASElementConfig.AccessibleDescription = null;
            this.pnlASElementConfig.AccessibleName = null;
            resources.ApplyResources(this.pnlASElementConfig, "pnlASElementConfig");
            this.pnlASElementConfig.BackgroundImage = null;
            this.pnlASElementConfig.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pnlASElementConfig.Font = null;
            this.pnlASElementConfig.Name = "pnlASElementConfig";
            // 
            // ctlASConfigElement
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.Controls.Add(this.pnlASElementConfig);
            this.Controls.Add(this.pnlButtons);
            this.Controls.Add(this.pnlTradeTypeSelection);
            this.Font = null;
            this.Name = "ctlASConfigElement";
            this.SizeChanged += new System.EventHandler(this.ctlASConfigElement_SizeChanged);
            this.pnlTradeTypeSelection.ResumeLayout(false);
            this.pnlTradeTypeSelection.PerformLayout();
            this.pnlButtons.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlTradeTypeSelection;
        private System.Windows.Forms.Label lblTradeType;
        private System.Windows.Forms.ComboBox cbxTradeType;
        private System.Windows.Forms.Panel pnlButtons;
        private System.Windows.Forms.Panel pnlASElementConfig;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnNew;
    }
}
