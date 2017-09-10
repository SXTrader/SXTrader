namespace net.sxtrader.bftradingstrategies.BFTSGUI
{
    partial class frmDonation
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmDonation));
            this.btnDonate = new System.Windows.Forms.Button();
            this.imgDonate = new System.Windows.Forms.ImageList(this.components);
            this.lblDonation = new System.Windows.Forms.Label();
            this.wbrUpdateInfo = new System.Windows.Forms.WebBrowser();
            this.btnClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnDonate
            // 
            resources.ApplyResources(this.btnDonate, "btnDonate");
            this.btnDonate.ImageList = this.imgDonate;
            this.btnDonate.Name = "btnDonate";
            this.btnDonate.UseVisualStyleBackColor = true;
            this.btnDonate.Click += new System.EventHandler(this.btnDonate_Click);
            // 
            // imgDonate
            // 
            this.imgDonate.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgDonate.ImageStream")));
            this.imgDonate.TransparentColor = System.Drawing.Color.Transparent;
            this.imgDonate.Images.SetKeyName(0, "Donate.jpg");
            this.imgDonate.Images.SetKeyName(1, "Donate2.jpg");
            this.imgDonate.Images.SetKeyName(2, "Donate3.jpg");
            // 
            // lblDonation
            // 
            resources.ApplyResources(this.lblDonation, "lblDonation");
            this.lblDonation.Name = "lblDonation";
            // 
            // wbrUpdateInfo
            // 
            resources.ApplyResources(this.wbrUpdateInfo, "wbrUpdateInfo");
            this.wbrUpdateInfo.Name = "wbrUpdateInfo";
            this.wbrUpdateInfo.Url = new System.Uri("", System.UriKind.Relative);
            this.wbrUpdateInfo.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.wbrUpdateInfo_Navigating);
            // 
            // btnClose
            // 
            resources.ApplyResources(this.btnClose, "btnClose");
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnClose.Name = "btnClose";
            this.btnClose.UseVisualStyleBackColor = true;
            // 
            // frmDonation
            // 
            this.AcceptButton = this.btnClose;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.wbrUpdateInfo);
            this.Controls.Add(this.lblDonation);
            this.Controls.Add(this.btnDonate);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmDonation";
            this.ShowIcon = false;
            this.Load += new System.EventHandler(this.frmDonation_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnDonate;
        private System.Windows.Forms.ImageList imgDonate;
        private System.Windows.Forms.Label lblDonation;
        private System.Windows.Forms.WebBrowser wbrUpdateInfo;
        private System.Windows.Forms.Button btnClose;
    }
}