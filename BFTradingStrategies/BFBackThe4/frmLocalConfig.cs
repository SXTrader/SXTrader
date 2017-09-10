using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace net.sxtrader.bftradingstrategies.BackThe4
{
    public partial class frmLocalConfig : Form
    {
        private ctlConfigBT4 ctlConfig;
        public frmLocalConfig()
        {
            InitializeComponent();
            doLanguage();
        }

        public BT4ConfigurationRW Configuration
        {
            set
            {
                ctlConfig = new ctlConfigBT4(value);
                this.pnlConfig.Controls.Clear();
                ctlConfig.Dock = DockStyle.Fill;
                ctlConfig.Width = pnlConfig.Width;
                ctlConfig.Height = pnlConfig.Height;
                this.pnlConfig.Controls.Add(ctlConfig);
            }
            get
            {
                return ctlConfig.Configuration;
            }

        }

        private void doLanguage()
        {
            this.Text = BackThe4.strLocalConfig;
            this.btnOK.Text = BackThe4.strOK;
            this.btnCancel.Text = BackThe4.strCancel;
        }

        private void frmLocalConfig_Load(object sender, EventArgs e)
        {            
        }
    }
}
