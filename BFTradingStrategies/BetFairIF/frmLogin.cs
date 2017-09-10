using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BetFairIF;

namespace net.sxtrader.bftradingstrategies.betfairif
{
    public partial class frmLogin : Form
    {
        public String User
        {
            get
            {
                return this.txtUser.Text;
            }
        }

        public String Pwd
        {
            get
            {
                return this.txtPwd.Text;
            }
        }

        public Boolean Remember
        {
            get
            {
                return this.cbxRemember.Checked;
            }
        }

        public frmLogin()
        {
            InitializeComponent();
            doLanguage();
        }

        private void doLanguage()
        {
            this.Text = BetFairIF.BetFairIF.strLoginCaption;
            lblUser.Text = BetFairIF.BetFairIF.strUser;
            lblPwd.Text = BetFairIF.BetFairIF.strPwd;
            cbxRemember.Text = BetFairIF.BetFairIF.strRememberQuestion;
            btnCancel.Text = BetFairIF.BetFairIF.strCancelBtn;
            btnLogin.Text = BetFairIF.BetFairIF.strLoginBtn;

            txtUser.Left = lblUser.Right + 5;
            txtPwd.Left = lblPwd.Right + 5;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
