using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using net.sxtrader.bftradingstrategies.ttr.Helper;
using net.sxtrader.bftradingstrategies.ttr.Configuration;

namespace net.sxtrader.bftradingstrategies.ttr.TradeModuls.CorrectScore.Controls
{
    public partial class ctlCSConfiguration : UserControl
    {
        private TTRConfigurationRW _config;

        public TTRConfigurationRW Configuration
        {
            set
            {
                _config = value;

                chkOnlyHedge.Checked            = _config.CSBackDefaultOnlyHedge;
                chkCheckLayOdds.Checked         = _config.CSBackDefaultCheckLayOdds;
                chkWaitTimes.Checked            = _config.CSBackDefaultUseWaitTime;
                chkUseHedgeWaittime.Checked     = _config.CSBackDefaultUseHedgeWaitTime;
                chkUseGreenWaittime.Checked     = _config.CSBackDefaultUseGreenWaitTime;
                chkNoTrade.Checked              = _config.NoTrade;
                chkOddsPercentage.Checked       = _config.CSBackDefaultUseOddsPercentage;
                rbnHedgeDynamic.Checked         = _config.CSBackDefaultUseHedgeSpecialPlayTime;
                cbxSpecialHedgePT.SelectedValue = _config.CSBackDefaultHedgeSpecialPlayime;
                rbnGreenDynamic.Checked         = _config.CSBackDefaultUseGreenSpecialPlayTime;
                cbxSpecialGreenPT.SelectedValue = _config.CSBackDefaultGreenSpecialPlayime;
                spnHedgeDelta.Value             = (decimal)_config.CSBackDefaultHedgeSpecialPlayimeDelta;
                spnHedgePlaytime.Value          = (decimal)_config.CSBackDefaultHedgePlayime;
                spnGreenDelta.Value             = (decimal)_config.CSBackDefaultGreenSpecialPlayimeDelta;
                spnGreenPlaytime.Value          = (decimal)_config.CSBackDefaultGreenPlayime;
                spnHedgeWaitTime.Value          = (decimal)_config.CSBackDefaultHedgeWaitTime;
                spnGreenWaitTime.Value          = (decimal)_config.CSBackDefaultGreenWaitTime;
                spnHedgePercentage.Value        = (decimal)_config.CSBackDefaultHedgePercentage;
                spnGreenPercentage.Value        = (decimal)_config.CSBackDefaultGreenPercentage;
            }
        }

        public ctlCSConfiguration()
        {
            InitializeComponent();
            TTRHelper.FillSpecialPlaytimeCombobox(cbxSpecialHedgePT);
            TTRHelper.FillSpecialPlaytimeCombobox(cbxSpecialGreenPT);
            
        }

        public void getConfig(ref TTRConfigurationRW config)
        {
            config.CSBackDefaultOnlyHedge                = chkOnlyHedge.Checked;
            config.CSBackDefaultCheckLayOdds             = chkCheckLayOdds.Checked;
            config.CSBackDefaultUseWaitTime              = chkWaitTimes.Checked;
            config.CSBackDefaultUseHedgeWaitTime         = chkUseHedgeWaittime.Checked;
            config.CSBackDefaultUseGreenWaitTime         = chkUseGreenWaittime.Checked;
            config.CSBackDefaultUseOddsPercentage        = chkOddsPercentage.Checked;
            config.CSBackDefaultUseHedgeSpecialPlayTime  = rbnHedgeDynamic.Checked;
            config.CSBackDefaultUseGreenSpecialPlayTime  = rbnGreenDynamic.Checked;
            config.NoTrade                               = chkNoTrade.Checked;

            config.CSBackDefaultHedgeSpecialPlayime      = (SPECIALPLAYTIME)Enum.Parse(typeof(SPECIALPLAYTIME), cbxSpecialHedgePT.SelectedValue.ToString());
            config.CSBackDefaultGreenSpecialPlayime      = (SPECIALPLAYTIME)Enum.Parse(typeof(SPECIALPLAYTIME), cbxSpecialGreenPT.SelectedValue.ToString());

            config.CSBackDefaultHedgeWaitTime            = (int)spnHedgeWaitTime.Value;
            config.CSBackDefaultGreenWaitTime            = (int)spnGreenWaitTime.Value;
            config.CSBackDefaultHedgePlayime             = (int)spnHedgePlaytime.Value;
            config.CSBackDefaultGreenPlayime             = (int)spnGreenPlaytime.Value;
            config.CSBackDefaultHedgeSpecialPlayimeDelta = (int)spnHedgeDelta.Value;
            config.CSBackDefaultGreenSpecialPlayimeDelta = (int)spnGreenDelta.Value;
            config.CSBackDefaultHedgePercentage          = (int)spnHedgePercentage.Value;
            config.CSBackDefaultGreenPercentage          = (int)spnGreenPercentage.Value;
        }
        

        

        private void ctlCSConfiguration_SizeChanged(object sender, EventArgs e)
        {
            gbxWaitTimes.Width = this.Width;
            gbxOddsPercentage.Width = this.Width;
        }

        private void chkNoTrade_CheckedChanged(object sender, EventArgs e)
        {
            bool bVal = false;
            if((sender as CheckBox).Checked)
            {
                bVal = false;
            }
            else
            {
                bVal = true;
            }

            chkOnlyHedge.Enabled        = bVal;
            chkCheckLayOdds.Enabled     = bVal;
            chkWaitTimes.Enabled        = bVal;
            chkOddsPercentage.Enabled   = bVal;
            chkUseGreenWaittime.Enabled = bVal;
            chkUseHedgeWaittime.Enabled = bVal;
            //chkNoTrade.Enabled          = bVal;
            cbxSpecialHedgePT.Enabled   = bVal;
            cbxSpecialGreenPT.Enabled   = bVal;
            spnHedgeWaitTime.Enabled    = bVal;
            spnGreenWaitTime.Enabled    = bVal;
            spnHedgePlaytime.Enabled    = bVal;
            spnGreenPlaytime.Enabled    = bVal;
            spnHedgeDelta.Enabled       = bVal;
            spnGreenDelta.Enabled       = bVal;
            spnHedgePercentage.Enabled  = bVal;
            spnGreenPercentage.Enabled  = bVal;
            rbnFixedGreenPT.Enabled     = bVal;
            rbnGreenDynamic.Enabled     = bVal;
            rbnFixedHedgePT.Enabled     = bVal;
            rbnHedgeDynamic.Enabled     = bVal;
        }
    }
}
