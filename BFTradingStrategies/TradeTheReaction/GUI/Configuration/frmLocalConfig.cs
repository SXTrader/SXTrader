using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using net.sxtrader.bftradingstrategies.ttr.Configuration;

namespace net.sxtrader.bftradingstrategies.ttr.GUI.Configuration
{
    public partial class frmLocalConfig : Form
    {
        private ctlConfiguration _ctlConfig;
        public TTRConfigurationRW Configuration
        {
            set
            {
                _ctlConfig = new ctlConfiguration(value);

                while(this.pnlConfig.Controls.Count > 0)
                {
                    this.pnlConfig.Controls[0].Dispose();
                }
                _ctlConfig.Dock = DockStyle.Fill;
                _ctlConfig.Width = pnlConfig.Width;
                _ctlConfig.Height = pnlConfig.Height;
                //_ctlConfig.HideSoundTab = true;
                this.pnlConfig.Controls.Add(_ctlConfig);
            }
            get
            {
                return _ctlConfig.Configuration;
            }

        }


        public frmLocalConfig()
        {
            InitializeComponent();
           
        }
    }
}
