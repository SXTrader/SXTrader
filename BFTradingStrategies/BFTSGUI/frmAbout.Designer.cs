namespace net.sxtrader.bftradingstrategies.BFTSGUI
{
    partial class frmAbout
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAbout));
            this.lblAbout = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tlpComponents = new System.Windows.Forms.TableLayoutPanel();
            this.llbImapX = new System.Windows.Forms.LinkLabel();
            this.llbImapXLicense = new System.Windows.Forms.LinkLabel();
            this.groupBox1.SuspendLayout();
            this.tlpComponents.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblAbout
            // 
            this.lblAbout.AutoSize = true;
            this.lblAbout.Location = new System.Drawing.Point(13, 13);
            this.lblAbout.Name = "lblAbout";
            this.lblAbout.Size = new System.Drawing.Size(93, 13);
            this.lblAbout.TabIndex = 0;
            this.lblAbout.Text = "SXTrader Version ";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tlpComponents);
            this.groupBox1.Location = new System.Drawing.Point(16, 30);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(367, 100);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Used Third Party Components";
            // 
            // tlpComponents
            // 
            this.tlpComponents.AutoSize = true;
            this.tlpComponents.ColumnCount = 2;
            this.tlpComponents.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpComponents.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpComponents.Controls.Add(this.llbImapXLicense, 1, 0);
            this.tlpComponents.Controls.Add(this.llbImapX, 0, 0);
            this.tlpComponents.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpComponents.Location = new System.Drawing.Point(3, 16);
            this.tlpComponents.Name = "tlpComponents";
            this.tlpComponents.RowCount = 2;
            this.tlpComponents.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpComponents.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpComponents.Size = new System.Drawing.Size(361, 81);
            this.tlpComponents.TabIndex = 0;
            // 
            // llbImapX
            // 
            this.llbImapX.AutoSize = true;
            this.llbImapX.Location = new System.Drawing.Point(3, 0);
            this.llbImapX.Name = "llbImapX";
            this.llbImapX.Size = new System.Drawing.Size(37, 13);
            this.llbImapX.TabIndex = 0;
            this.llbImapX.TabStop = true;
            this.llbImapX.Text = "ImapX";
            this.llbImapX.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.llbImapX.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llbImapX_LinkClicked);
            // 
            // llbImapXLicense
            // 
            this.llbImapXLicense.AutoSize = true;
            this.llbImapXLicense.Location = new System.Drawing.Point(183, 0);
            this.llbImapXLicense.Name = "llbImapXLicense";
            this.llbImapXLicense.Size = new System.Drawing.Size(44, 13);
            this.llbImapXLicense.TabIndex = 1;
            this.llbImapXLicense.TabStop = true;
            this.llbImapXLicense.Text = "License";
            this.llbImapXLicense.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.llbImapXLicense.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llbImapXLicense_LinkClicked);
            // 
            // frmAbout
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(395, 138);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lblAbout);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmAbout";
            this.Text = "About";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tlpComponents.ResumeLayout(false);
            this.tlpComponents.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblAbout;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TableLayoutPanel tlpComponents;
        private System.Windows.Forms.LinkLabel llbImapXLicense;
        private System.Windows.Forms.LinkLabel llbImapX;
    }
}