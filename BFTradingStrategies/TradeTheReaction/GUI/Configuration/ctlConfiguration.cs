using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using net.sxtrader.plugin;
using net.sxtrader.bftradingstrategies.ttr.Configuration;
using net.sxtrader.bftradingstrategies.ttr.TradeModuls.OverUnder.Controls;
using net.sxtrader.bftradingstrategies.ttr.TradeModuls.CorrectScore.Controls;

namespace net.sxtrader.bftradingstrategies.ttr.GUI.Configuration
{
    public partial class ctlConfiguration : UserControl, IConfiguration
    {
        private ctlSL00Configuration _ctlSL00cfg;
        private ctlFBConfiguration _ctlFBcfg;
        private ctlGenConfiguration _ctlGenCfg;
        private ctlOUConfiguration _ctlOUCfg;
        private ctlCSConfiguration _ctlCSCfg;
        private TTRConfigurationRW _config;

        public TTRConfigurationRW Configuration
        {
            get 
            {
                _ctlSL00cfg.getConfig(ref _config);
                _ctlFBcfg.getConfig(ref _config);
                _ctlGenCfg.getConfig(ref _config);
                _ctlOUCfg.getConfig(ref _config);
                _ctlCSCfg.getConfig(ref _config);

                return _config; 
            }
            set
            {
                _config = value;
                if (_config != null)
                {
                    _ctlSL00cfg.Configuration = _config;
                    _ctlFBcfg.Configuration   = _config;
                    _ctlGenCfg.Configuration  = _config;
                    _ctlOUCfg.Configuration   = _config;
                    _ctlCSCfg.Configuration = _config;
                }
            }

        }

        public ctlConfiguration()
        {

            InitializeComponent();
            _ctlSL00cfg = new ctlSL00Configuration();
            _ctlSL00cfg.Dock = DockStyle.Fill;
            tbpSL00.Controls.Add(_ctlSL00cfg);

            _ctlFBcfg = new ctlFBConfiguration();
            _ctlFBcfg.Dock = DockStyle.Fill;
            tbpMoney.Controls.Add(_ctlFBcfg);

            _ctlGenCfg = new ctlGenConfiguration();
            _ctlGenCfg.Dock = DockStyle.Fill;
            tbpGeneral.Controls.Add(_ctlGenCfg);

            _ctlOUCfg = new ctlOUConfiguration();
            _ctlOUCfg.Dock = DockStyle.Fill;
            tbpOverUnder.Controls.Add(_ctlOUCfg);

            _ctlCSCfg = new ctlCSConfiguration();
            _ctlCSCfg.Dock = DockStyle.Fill;
            tbpCorrectScore.Controls.Add(_ctlCSCfg);

            _config = new TTRConfigurationRW();
            _ctlSL00cfg.Configuration = _config;
            _ctlFBcfg.Configuration   = _config;
            _ctlGenCfg.Configuration  = _config;
            _ctlOUCfg.Configuration   = _config;
            _ctlCSCfg.Configuration   = _config;
        }

        public ctlConfiguration(TTRConfigurationRW config)
        {
            InitializeComponent();

            _ctlSL00cfg = new ctlSL00Configuration();
            _ctlSL00cfg.Dock = DockStyle.Fill;
            tbpSL00.Controls.Add(_ctlSL00cfg);

            _ctlFBcfg = new ctlFBConfiguration();
            _ctlFBcfg.Dock = DockStyle.Fill;
            tbpMoney.Controls.Add(_ctlFBcfg);

            _ctlGenCfg = new ctlGenConfiguration();
            _ctlGenCfg.Dock = DockStyle.Fill;
            tbpGeneral.Controls.Add(_ctlGenCfg);

            _ctlOUCfg = new ctlOUConfiguration();
            _ctlOUCfg.Dock = DockStyle.Fill;
            tbpOverUnder.Controls.Add(_ctlOUCfg);

            _ctlCSCfg = new ctlCSConfiguration();
            _ctlCSCfg.Dock = DockStyle.Fill;
            tbpCorrectScore.Controls.Add(_ctlCSCfg);

            this.Configuration = config;
        }

        #region IConfiguration Member

        public void save()
        {
            //throw new NotImplementedException();
            _ctlSL00cfg.getConfig(ref _config);
            _ctlFBcfg.getConfig(ref _config);
            _ctlGenCfg.getConfig(ref _config);
            _ctlOUCfg.getConfig(ref _config);
            _ctlCSCfg.getConfig(ref _config);
            _config.save();
        }

        public string getXML()
        {
            return _config.getXML();
        }

        #endregion
    }
}
