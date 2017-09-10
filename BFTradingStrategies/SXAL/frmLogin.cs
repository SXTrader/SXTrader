using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using net.sxtrader.bftradingstrategies.SXALInterfaces;

namespace net.sxtrader.bftradingstrategies.SXAL
{
    public partial class frmLogin : Form
    {
        public frmLogin()
        {
            InitializeComponent();
        }

        public EXCHANGES Exchange
        {
            get
            {
                EXCHANGES selectedExchange = EXCHANGES.UNASSIGNED;
                if (rbnBetfair.Checked)
                    selectedExchange = EXCHANGES.BETFAIR;
                else if (rbnBetdaqCom.Checked)
                    selectedExchange = EXCHANGES.BETDAQCOM;
                else if (rbnBetfairUK.Checked)
                    selectedExchange = EXCHANGES.BETDAQUK;
                else if (rbnPinBet88.Checked)
                    selectedExchange = EXCHANGES.PINBET88;

                return selectedExchange;
            }
        }

        public String Pwd { get { return txtPwd.Text; } }
        public String Usr { get { return txtUser.Text; } }

        private void rbn_CheckedChanged(object sender, EventArgs e)
        {
            if (rbnBetfair.Checked == true || rbnBetdaqCom.Checked == true || rbnBetfairUK.Checked == true || rbnPinBet88.Checked == true)
            {
                btnLogin.Enabled = true;
            }
            else
            {
                btnLogin.Enabled = false;
            }
        }

        private void lnkBetfair_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://ads.betfair.com/redirect.aspx?pid=69255&bid=3797");
        }

        private void lnkBetdaq_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://affiliate.cdn.betdaqaffiliates.com/redirect.aspx?pid=3045&bid=1820");
        }

        private void lnkBetdaqUK_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://affiliate.cdn.betdaqaffiliates.com/redirect.aspx?pid=3045&bid=1820");
        }
    }
}
