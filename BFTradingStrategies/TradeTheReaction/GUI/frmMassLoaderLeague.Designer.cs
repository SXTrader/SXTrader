namespace net.sxtrader.bftradingstrategies.ttr.GUI
{
    partial class frmMassLoaderLeague
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMassLoaderLeague));
            this.clbLeagues = new System.Windows.Forms.CheckedListBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.txtTemplate = new System.Windows.Forms.TextBox();
            this.btnLoadDialog = new System.Windows.Forms.Button();
            this.ofdTemplate = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // clbLeagues
            // 
            this.clbLeagues.AccessibleDescription = null;
            this.clbLeagues.AccessibleName = null;
            resources.ApplyResources(this.clbLeagues, "clbLeagues");
            this.clbLeagues.BackgroundImage = null;
            this.clbLeagues.Font = null;
            this.clbLeagues.FormattingEnabled = true;
            this.clbLeagues.Name = "clbLeagues";
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
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
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
            // txtTemplate
            // 
            this.txtTemplate.AccessibleDescription = null;
            this.txtTemplate.AccessibleName = null;
            resources.ApplyResources(this.txtTemplate, "txtTemplate");
            this.txtTemplate.BackgroundImage = null;
            this.txtTemplate.Font = null;
            this.txtTemplate.Name = "txtTemplate";
            this.txtTemplate.ReadOnly = true;
            // 
            // btnLoadDialog
            // 
            this.btnLoadDialog.AccessibleDescription = null;
            this.btnLoadDialog.AccessibleName = null;
            resources.ApplyResources(this.btnLoadDialog, "btnLoadDialog");
            this.btnLoadDialog.BackgroundImage = null;
            this.btnLoadDialog.Font = null;
            this.btnLoadDialog.Name = "btnLoadDialog";
            this.btnLoadDialog.UseVisualStyleBackColor = true;
            this.btnLoadDialog.Click += new System.EventHandler(this.btnLoadDialog_Click);
            // 
            // ofdTemplate
            // 
            resources.ApplyResources(this.ofdTemplate, "ofdTemplate");
            // 
            // frmMassLoaderLeague
            // 
            this.AcceptButton = this.btnOK;
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.CancelButton = this.btnCancel;
            this.ControlBox = false;
            this.Controls.Add(this.btnLoadDialog);
            this.Controls.Add(this.txtTemplate);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.clbLeagues);
            this.Font = null;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "frmMassLoaderLeague";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckedListBox clbLeagues;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox txtTemplate;
        private System.Windows.Forms.Button btnLoadDialog;
        private System.Windows.Forms.OpenFileDialog ofdTemplate;
    }
}