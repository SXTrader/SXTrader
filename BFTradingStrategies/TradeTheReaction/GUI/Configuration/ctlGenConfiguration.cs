using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using net.sxtrader.bftradingstrategies.ttr.Configuration;

namespace net.sxtrader.bftradingstrategies.ttr.GUI.Configuration
{
    public partial class ctlGenConfiguration : UserControl
    {
        private TTRConfigurationRW _config;

        public TTRConfigurationRW Configuration
        {
            set
            {
                _config = value;

                spnStartMinutes.Value = (decimal)_config.PreplayStartPoint;

            }
        }

        public ctlGenConfiguration()
        {
            InitializeComponent();            
        }

        public void getConfig(ref TTRConfigurationRW config)
        {
            config.PreplayStartPoint = (int)spnStartMinutes.Value;
        }        
    }
}
