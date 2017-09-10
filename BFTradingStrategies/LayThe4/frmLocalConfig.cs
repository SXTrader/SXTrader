using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace net.sxtrader.bftradingstrategies.LayThe4
{
    public partial class frmLocalConfig : Form
    {
        private ctlConfigLT4 ctlConfig;

        public frmLocalConfig()
        {
            InitializeComponent();
            doLanguage();
        }

        public LT4ConfigurationRW Configuration
        {
            set
            {
                ctlConfig = new ctlConfigLT4(value);
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
            this.Text = LayThe4.strLocalConfig;
            this.btnOK.Text = LayThe4.strOK;
            this.btnCancel.Text = LayThe4.strCancel;
        }
    }
}
