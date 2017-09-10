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
    public partial class frmUpdateInfo : Form
    {
        private String _updateInfoUrl;
        public frmUpdateInfo()
        {
            InitializeComponent();
            lblInfo.Text = "There's a new version of SXTrader ready to download.\r\n";
        }

        public String UpdateInfo
        {
            set
            {
                
                _updateInfoUrl = value;
                wbrUpdateInfo.Url = new Uri(_updateInfoUrl);
                //wbrUpdateInfo.Navigate(_updateInfoUrl);
            }
        }
    }
}
