using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using net.sxtrader.bftradingstrategies.ttr.Configuration;

namespace net.sxtrader.bftradingstrategies.ttr.TradeModuls.OverUnder.Controls
{
    public partial class ctlOUConfiguration : UserControl
    {
        private TTRConfigurationRW _config;

        public TTRConfigurationRW Configuration
        {
            set
            {
                _config = value;

                chkOnlyHedge.Checked = _config.OverUnderDefaultOnlyHedge;
                chkCheckLayOdds.Checked = _config.OverUnderDefaultCheckLayOdds;
                chkWaitTimes.Checked = _config.OverUnderDefaultUseWaitTime;
                chkUseHedgeWaittime.Checked = _config.OverUnderDefaultUseHedgeWaittime;
                chkUseGreenWaittime.Checked = _config.OverUnderDefaultUseGreenWaittime;
                chkNoTrade.Checked = _config.NoTrade;
                chkOddsPercentage.Checked = _config.OverUnderDefaultUseOddsPercentage;
                /*
                rbnHedgeDynamic.Checked = _config.UseHedgeSpecialPlayTime;
                cbxSpecialHedgePT.SelectedValue = _config.HedgeSpecialPlayime;
                rbnGreenDynamic.Checked = _config.UseGreenSpecialPlayTime;
                cbxSpecialGreenPT.SelectedValue = _config.GreenSpecialPlayime;
                spnHedgeDelta.Value = (decimal)_config.HedgeSpecialPlayimeDelta;
                 */
                spnHedgePlaytime.Value = (decimal)_config.OverUnderDefaultHedgePlayime;
                //spnGreenDelta.Value = (decimal)_config.GreenSpecialPlayimeDelta;
                spnGreenPlaytime.Value = (decimal)_config.OverUnderDefaultGreenPlayime;
                spnHedgeWaitTime.Value = (decimal)_config.OverUnderDefaultHedgeWaitTime;
                spnGreenWaitTime.Value = (decimal)_config.OverUnderDefaultGreenWaitTime;
                spnHedgePercentage.Value = (decimal)_config.OverUnderDefaultHedgePercentage;
                spnGreenPercentage.Value = (decimal)_config.OverUnderDefaultGreenPercentage;
            }
        }

        public ctlOUConfiguration()
        {
            InitializeComponent();
        }

        public void getConfig(ref TTRConfigurationRW config)
        {
            config.OverUnderDefaultOnlyHedge         = chkOnlyHedge.Checked;
            config.OverUnderDefaultCheckLayOdds      = chkCheckLayOdds.Checked;
            config.OverUnderDefaultUseWaitTime       = chkWaitTimes.Checked;
            config.OverUnderDefaultUseHedgeWaittime  = chkUseHedgeWaittime.Checked;
            config.OverUnderDefaultUseGreenWaittime  = chkUseGreenWaittime.Checked;
            config.OverUnderDefaultUseOddsPercentage = chkOddsPercentage.Checked;
            //config.UseHedgeSpecialPlayTime = rbnHedgeDynamic.Checked;
            //config.UseGreenSpecialPlayTime = rbnGreenDynamic.Checked;
            config.NoTrade                           = chkNoTrade.Checked;

            //config.HedgeSpecialPlayime = (SPECIALPLAYTIME)Enum.Parse(typeof(SPECIALPLAYTIME), cbxSpecialHedgePT.SelectedValue.ToString());
            //config.GreenSpecialPlayime = (SPECIALPLAYTIME)Enum.Parse(typeof(SPECIALPLAYTIME), cbxSpecialGreenPT.SelectedValue.ToString());

            config.OverUnderDefaultHedgeWaitTime   = (int)spnHedgeWaitTime.Value;
            config.OverUnderDefaultGreenWaitTime   = (int)spnGreenWaitTime.Value;
            config.OverUnderDefaultHedgePlayime    = (int)spnHedgePlaytime.Value;
            config.OverUnderDefaultGreenPlayime    = (int)spnGreenPlaytime.Value;
            //config.HedgeSpecialPlayimeDelta = (int)spnHedgeDelta.Value;
            //config.GreenSpecialPlayimeDelta = (int)spnGreenDelta.Value;
            config.OverUnderDefaultHedgePercentage = (int)spnHedgePercentage.Value;
            config.OverUnderDefaultGreenPercentage = (int)spnGreenPercentage.Value;
        }

        private void chkNoTrade_CheckedChanged(object sender, EventArgs e)
        {
            bool bVal = false;
            if ((sender as CheckBox).Checked)
            {
                bVal = false;
            }
            else
            {
                bVal = true;
            }

            chkOnlyHedge.Enabled = bVal;
            chkCheckLayOdds.Enabled = bVal;
            chkWaitTimes.Enabled = bVal;
            chkOddsPercentage.Enabled = bVal;
            chkUseGreenWaittime.Enabled = bVal;
            chkUseHedgeWaittime.Enabled = bVal;
            //chkNoTrade.Enabled          = bVal;
            //cbxSpecialHedgePT.Enabled = bVal;
            //cbxSpecialGreenPT.Enabled = bVal;
            spnHedgeWaitTime.Enabled = bVal;
            spnGreenWaitTime.Enabled = bVal;
            spnHedgePlaytime.Enabled = bVal;
            spnGreenPlaytime.Enabled = bVal;
            //spnHedgeDelta.Enabled = bVal;
            //spnGreenDelta.Enabled = bVal;
            spnHedgePercentage.Enabled = bVal;
            spnGreenPercentage.Enabled = bVal;
            //rbnFixedGreenPT.Enabled = bVal;
            //rbnGreenDynamic.Enabled = bVal;
            //rbnFixedHedgePT.Enabled = bVal;
            //rbnHedgeDynamic.Enabled = bVal;
        }
    }
}
