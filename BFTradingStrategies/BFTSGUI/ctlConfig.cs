using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using net.sxtrader.plugin;

namespace net.sxtrader.bftradingstrategies.BFTSGUI
{
    public partial class ctlConfig : UserControl, IConfiguration
    {
        private GenConfigurationRW m_config;
        public ctlConfig()
        {
            m_config = new GenConfigurationRW();
            InitializeComponent();            
            loadConfig();
        }

        private void loadConfig()
        {
            spnLiveSeconds.Value = m_config.LivetickerCheck;
            spnBFSeconds.Value = m_config.BetfairCheck;
            cbxWriteDebug.Checked = m_config.WriteDebugFile;
            cbxConfirmFastBet.Checked = m_config.ConfirmFastBet;
            cbxLogLiveticker.Checked = m_config.LogLiveticker;
            cbxLogTrades.Checked = m_config.LogTrades;
            cbxLogBetAmounts.Checked = m_config.LogBetAmounts;
        }
        

        #region IConfiguration Member

        public void save()
        {
            m_config.BetfairCheck = (int)spnBFSeconds.Value;
            m_config.LivetickerCheck = (int)spnLiveSeconds.Value;
            m_config.WriteDebugFile = cbxWriteDebug.Checked;
            m_config.ConfirmFastBet = cbxConfirmFastBet.Checked;
            m_config.LogLiveticker = cbxLogLiveticker.Checked;
            m_config.LogTrades = cbxLogTrades.Checked;
            m_config.LogBetAmounts = cbxLogBetAmounts.Checked;
            m_config.save();
        }

        #endregion

        #region IConfiguration Member


        public string getXML()
        {
            return String.Empty;
        }

        #endregion
    }
}
