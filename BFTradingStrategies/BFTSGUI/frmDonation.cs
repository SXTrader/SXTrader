using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Globalization;

namespace net.sxtrader.bftradingstrategies.BFTSGUI
{
    public partial class frmDonation : Form
    {
        private System.Threading.Thread _makeDonationThread;
        public frmDonation()
        {
            InitializeComponent();
            _makeDonationThread = new System.Threading.Thread(this.makeDonation);
            try
            {
                /*
                WebRequest req = WebRequest.Create(Resources.strStartUpUrl);

                HttpWebResponse response = (HttpWebResponse)req.GetResponse();
                 */
                wbrUpdateInfo.Url = new Uri(Resources.strStartUpUrl);

                 
                
            }
            catch (System.Net.WebException)
            {
                wbrUpdateInfo.Visible = false;
                this.Height = 220;
            }
        }

        private void frmDonation_Load(object sender, EventArgs e)
        {            
            
        }

        private void btnDonate_Click(object sender, EventArgs e)
        {
            _makeDonationThread.Start();
        }

        private void makeDonation()
        {
            System.Diagnostics.Process.Start("https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=" + "TKJNJ54TU5ZAJ");
        }

        private void wbrUpdateInfo_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            if (e.Url != new Uri(Resources.strStartUpUrl))
            {
                wbrUpdateInfo.Stop();
                wbrUpdateInfo.Url = new Uri("http://www.sxtrader.net/SXTraderStartInfo.html");
                System.Diagnostics.Process.Start(e.Url.ToString());                
            }
        }
    }
}
