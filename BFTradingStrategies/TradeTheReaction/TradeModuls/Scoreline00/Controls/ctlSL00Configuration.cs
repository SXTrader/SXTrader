using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using net.sxtrader.bftradingstrategies.ttr.Configuration;
using net.sxtrader.plugin;
using net.sxtrader.bftradingstrategies.ttr.Helper;

namespace net.sxtrader.bftradingstrategies.ttr.GUI.Configuration
{
    public partial class ctlSL00Configuration : UserControl
    {
        private TTRConfigurationRW _config;

        public TTRConfigurationRW Configuration
        {
            set
            {
                _config = value;

                chkOnlyHedge.Checked            = _config.OnlyHedge;
                chkCheckLayOdds.Checked         = _config.CheckLayOdds;
                chkWaitTimes.Checked            = _config.UseWaitTime;
                chkUseHedgeWaittime.Checked     = _config.UseHedgeWaitTime;
                chkUseGreenWaittime.Checked     = _config.UseGreenWaitTime;
                chkNoTrade.Checked              = _config.NoTrade;
                chkOddsPercentage.Checked       = _config.UseOddsPercentage;
                rbnHedgeDynamic.Checked         = _config.UseHedgeSpecialPlayTime;
                cbxSpecialHedgePT.SelectedValue = _config.HedgeSpecialPlayime;
                rbnGreenDynamic.Checked         = _config.UseGreenSpecialPlayTime;
                cbxSpecialGreenPT.SelectedValue = _config.GreenSpecialPlayime;
                spnHedgeDelta.Value             = (decimal)_config.HedgeSpecialPlayimeDelta;
                spnHedgePlaytime.Value          = (decimal)_config.HedgePlayime;
                spnGreenDelta.Value             = (decimal)_config.GreenSpecialPlayimeDelta;
                spnGreenPlaytime.Value          = (decimal)_config.GreenPlayime;
                spnHedgeWaitTime.Value          = (decimal)_config.HedgeWaitTime;
                spnGreenWaitTime.Value          = (decimal)_config.GreenWaitTime;
                spnHedgePercentage.Value        = (decimal)_config.HedgePercentage;
                spnGreenPercentage.Value        = (decimal)_config.GreenPercentage;
            }
        }

        public ctlSL00Configuration()
        {
            InitializeComponent();            
            TTRHelper.FillSpecialPlaytimeCombobox(cbxSpecialHedgePT);
            TTRHelper.FillSpecialPlaytimeCombobox(cbxSpecialGreenPT);
            
        }

        public void getConfig(ref TTRConfigurationRW config)
        {
            config.OnlyHedge                = chkOnlyHedge.Checked;
            config.CheckLayOdds             = chkCheckLayOdds.Checked;
            config.UseWaitTime              = chkWaitTimes.Checked;
            config.UseHedgeWaitTime         = chkUseHedgeWaittime.Checked;
            config.UseGreenWaitTime         = chkUseGreenWaittime.Checked;
            config.UseOddsPercentage        = chkOddsPercentage.Checked;
            config.UseHedgeSpecialPlayTime  = rbnHedgeDynamic.Checked;
            config.UseGreenSpecialPlayTime  = rbnGreenDynamic.Checked;
            config.NoTrade                  = chkNoTrade.Checked;

            config.HedgeSpecialPlayime      = (SPECIALPLAYTIME)Enum.Parse(typeof(SPECIALPLAYTIME), cbxSpecialHedgePT.SelectedValue.ToString());
            config.GreenSpecialPlayime      = (SPECIALPLAYTIME)Enum.Parse(typeof(SPECIALPLAYTIME), cbxSpecialGreenPT.SelectedValue.ToString());

            config.HedgeWaitTime            = (int)spnHedgeWaitTime.Value;
            config.GreenWaitTime            = (int)spnGreenWaitTime.Value;
            config.HedgePlayime             = (int)spnHedgePlaytime.Value;
            config.GreenPlayime             = (int)spnGreenPlaytime.Value;
            config.HedgeSpecialPlayimeDelta = (int)spnHedgeDelta.Value;
            config.GreenSpecialPlayimeDelta = (int)spnGreenDelta.Value;
            config.HedgePercentage          = (int)spnHedgePercentage.Value;
            config.GreenPercentage          = (int)spnGreenPercentage.Value;
        }
             
        private void ctlSL00Configuration_SizeChanged(object sender, EventArgs e)
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
