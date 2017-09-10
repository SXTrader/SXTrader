using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using net.sxtrader.bftradingstrategies.bfuestrategy.controls;

namespace net.sxtrader.bftradingstrategies.bfuestrategy
{
    public partial class frmLocalConfig : Form
    {
        private ctlConfiguration ctlConfig;
        public frmLocalConfig()
        {
            InitializeComponent();            
        }

        public LTDConfigurationRW Configuration
        {
            set
            {
                ctlConfig = new ctlConfiguration(value);

                foreach (Control ctrl in this.pnlConfig.Controls)
                {
                    this.pnlConfig.Controls.Remove(ctrl);
                    ctrl.Dispose();
                }
                ctlConfig.Dock = DockStyle.Fill;
                ctlConfig.Width = pnlConfig.Width;
                ctlConfig.Height = pnlConfig.Height;
                ctlConfig.HideSoundTab = true;
                this.pnlConfig.Controls.Add(ctlConfig);
            }
            get
            {
                return ctlConfig.Configuration;
            }

        }        
    }
}
