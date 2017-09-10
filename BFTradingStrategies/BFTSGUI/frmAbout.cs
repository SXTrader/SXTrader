using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace net.sxtrader.bftradingstrategies.BFTSGUI
{
    public partial class frmAbout : Form
    {
        public frmAbout()
        {
            InitializeComponent();
            lblAbout.Text = String.Format(Resources.strSXTraderVersion, Application.ProductVersion);

            
            llbImapX.Links.Add(0, llbImapX.Text.Length, "http://imapx.codeplex.com/");
            llbImapXLicense.Links.Add(0, llbImapXLicense.Text.Length, "http://imapx.codeplex.com/license");
        }

        private void llbImapX_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Link.LinkData.ToString());
        }

        private void llbImapXLicense_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Link.LinkData.ToString());
        }
    }
}
