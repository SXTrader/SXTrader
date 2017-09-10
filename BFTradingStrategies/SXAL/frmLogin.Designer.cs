namespace net.sxtrader.bftradingstrategies.SXAL
{
    partial class frmLogin
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmLogin));
            this.gbxExchanges = new System.Windows.Forms.GroupBox();
            this.rbnBetfairUK = new System.Windows.Forms.RadioButton();
            this.lnkBetdaqCom = new System.Windows.Forms.LinkLabel();
            this.lnkBetfair = new System.Windows.Forms.LinkLabel();
            this.rbnBetdaqCom = new System.Windows.Forms.RadioButton();
            this.rbnBetfair = new System.Windows.Forms.RadioButton();
            this.pnlButtons = new System.Windows.Forms.Panel();
            this.btnLogin = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.pnlData = new System.Windows.Forms.Panel();
            this.txtPwd = new System.Windows.Forms.TextBox();
            this.lblPwd = new System.Windows.Forms.Label();
            this.txtUser = new System.Windows.Forms.TextBox();
            this.lblUser = new System.Windows.Forms.Label();
            this.rbnPinBet88 = new System.Windows.Forms.RadioButton();
            this.gbxExchanges.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            this.pnlData.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbxExchanges
            // 
            resources.ApplyResources(this.gbxExchanges, "gbxExchanges");
            this.gbxExchanges.Controls.Add(this.rbnPinBet88);
            this.gbxExchanges.Controls.Add(this.rbnBetfairUK);
            this.gbxExchanges.Controls.Add(this.lnkBetdaqCom);
            this.gbxExchanges.Controls.Add(this.lnkBetfair);
            this.gbxExchanges.Controls.Add(this.rbnBetdaqCom);
            this.gbxExchanges.Controls.Add(this.rbnBetfair);
            this.gbxExchanges.Name = "gbxExchanges";
            this.gbxExchanges.TabStop = false;
            // 
            // rbnBetfairUK
            // 
            resources.ApplyResources(this.rbnBetfairUK, "rbnBetfairUK");
            this.rbnBetfairUK.Name = "rbnBetfairUK";
            this.rbnBetfairUK.TabStop = true;
            this.rbnBetfairUK.UseVisualStyleBackColor = true;
            this.rbnBetfairUK.CheckedChanged += new System.EventHandler(this.rbn_CheckedChanged);
            // 
            // lnkBetdaqCom
            // 
            resources.ApplyResources(this.lnkBetdaqCom, "lnkBetdaqCom");
            this.lnkBetdaqCom.Name = "lnkBetdaqCom";
            this.lnkBetdaqCom.TabStop = true;
            this.lnkBetdaqCom.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkBetdaq_LinkClicked);
            // 
            // lnkBetfair
            // 
            resources.ApplyResources(this.lnkBetfair, "lnkBetfair");
            this.lnkBetfair.Name = "lnkBetfair";
            this.lnkBetfair.TabStop = true;
            this.lnkBetfair.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkBetfair_LinkClicked);
            // 
            // rbnBetdaqCom
            // 
            resources.ApplyResources(this.rbnBetdaqCom, "rbnBetdaqCom");
            this.rbnBetdaqCom.Name = "rbnBetdaqCom";
            this.rbnBetdaqCom.TabStop = true;
            this.rbnBetdaqCom.UseVisualStyleBackColor = true;
            this.rbnBetdaqCom.CheckedChanged += new System.EventHandler(this.rbn_CheckedChanged);
            // 
            // rbnBetfair
            // 
            resources.ApplyResources(this.rbnBetfair, "rbnBetfair");
            this.rbnBetfair.Name = "rbnBetfair";
            this.rbnBetfair.TabStop = true;
            this.rbnBetfair.UseVisualStyleBackColor = true;
            this.rbnBetfair.CheckedChanged += new System.EventHandler(this.rbn_CheckedChanged);
            // 
            // pnlButtons
            // 
            resources.ApplyResources(this.pnlButtons, "pnlButtons");
            this.pnlButtons.Controls.Add(this.btnLogin);
            this.pnlButtons.Controls.Add(this.btnCancel);
            this.pnlButtons.Name = "pnlButtons";
            // 
            // btnLogin
            // 
            resources.ApplyResources(this.btnLogin, "btnLogin");
            this.btnLogin.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // pnlData
            // 
            resources.ApplyResources(this.pnlData, "pnlData");
            this.pnlData.Controls.Add(this.txtPwd);
            this.pnlData.Controls.Add(this.lblPwd);
            this.pnlData.Controls.Add(this.txtUser);
            this.pnlData.Controls.Add(this.lblUser);
            this.pnlData.Name = "pnlData";
            // 
            // txtPwd
            // 
            resources.ApplyResources(this.txtPwd, "txtPwd");
            this.txtPwd.Name = "txtPwd";
            // 
            // lblPwd
            // 
            resources.ApplyResources(this.lblPwd, "lblPwd");
            this.lblPwd.Name = "lblPwd";
            // 
            // txtUser
            // 
            resources.ApplyResources(this.txtUser, "txtUser");
            this.txtUser.Name = "txtUser";
            // 
            // lblUser
            // 
            resources.ApplyResources(this.lblUser, "lblUser");
            this.lblUser.Name = "lblUser";
            // 
            // rbnPinBet88
            // 
            resources.ApplyResources(this.rbnPinBet88, "rbnPinBet88");
            this.rbnPinBet88.Name = "rbnPinBet88";
            this.rbnPinBet88.TabStop = true;
            this.rbnPinBet88.UseVisualStyleBackColor = true;
            // 
            // frmLogin
            // 
            this.AcceptButton = this.btnLogin;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ControlBox = false;
            this.Controls.Add(this.pnlData);
            this.Controls.Add(this.pnlButtons);
            this.Controls.Add(this.gbxExchanges);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmLogin";
            this.gbxExchanges.ResumeLayout(false);
            this.gbxExchanges.PerformLayout();
            this.pnlButtons.ResumeLayout(false);
            this.pnlButtons.PerformLayout();
            this.pnlData.ResumeLayout(false);
            this.pnlData.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbxExchanges;
        private System.Windows.Forms.Panel pnlButtons;
        private System.Windows.Forms.Panel pnlData;
        private System.Windows.Forms.RadioButton rbnBetfair;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.RadioButton rbnBetdaqCom;
        private System.Windows.Forms.TextBox txtPwd;
        private System.Windows.Forms.Label lblPwd;
        private System.Windows.Forms.TextBox txtUser;
        private System.Windows.Forms.Label lblUser;
        private System.Windows.Forms.LinkLabel lnkBetfair;
        private System.Windows.Forms.LinkLabel lnkBetdaqCom;
        private System.Windows.Forms.RadioButton rbnBetfairUK;
        private System.Windows.Forms.RadioButton rbnPinBet88;
    }
}