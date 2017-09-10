using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using net.sxtrader.bftradingstrategies.ttr.Configuration;
using net.sxtrader.bftradingstrategies.ttr.TradeRules;
using net.sxtrader.bftradingstrategies.tradeinterfaces;

namespace net.sxtrader.bftradingstrategies.ttr.GUI.Configuration
{
    public partial class ctlTradeOutRuleEditOver : ctlTradeOutRuleEditAbstract//UserControl, ITradeOutRuleEdit
    {
        

        //public event EventHandler<TTRTOElementSaveEventArgs> SaveTOElement;

        public override TTRTradeOutCheck TradeOutCheck
        {
            get
            {
                String str = cbxTrigger.SelectedValue.ToString();                
                _tradeOutCheck.Trigger = (TRADEOUTTRIGGER)Enum.Parse(typeof(TRADEOUTTRIGGER), str);
                _tradeOutCheck.Order = (int)spnCheckOrder.Value;

                //Überprüfungsregeln
                getGoalSumRule(chkCheckGoalSum.Checked, (int)spnGoalSumHi.Value, (int)spnGoalSumLo.Value);
                getPlaytimeRule(chkCheckPlaytime.Checked, (int)spnPlaytimeHi.Value, (int)spnPlaytimeLo.Value);
                getRedCardRule(rbnRCEqual.Checked, rbnRCTeamA.Checked, rbnRCTeamB.Checked);

                // Over kennt kein Correct Score
                int iCounter = 0;
                while (iCounter < _tradeOutCheck.Rules.Count)
                {
                    TTRTradeOutRuleFragment fragment = _tradeOutCheck.Rules[iCounter];
                    if (fragment.GetType() == typeof(TTRTradeOutScoreRule))
                    {
                        _tradeOutCheck.Rules.Remove(fragment);
                        break;
                    }
                    else iCounter++;
                }

                //Trade Out Settings
                getTradeOutSettings();
   
                return _tradeOutCheck;
            }
            set
            {
                _tradeOutCheck = value;
                if (_tradeOutCheck != null)
                {
                    cbxTrigger.Enabled = spnCheckOrder.Enabled = chkCheckPlaytime.Enabled = spnPlaytimeLo.Enabled = spnPlaytimeHi.Enabled =
                                    chkCheckGoalSum.Enabled = spnGoalSumLo.Enabled = spnGoalSumHi.Enabled = rbnNoRedCard.Enabled = rbnRCEqual.Enabled =
                                    rbnRCTeamA.Enabled = rbnRCTeamB.Enabled = chkOnlyHedge.Enabled = chkCheckLayOdds.Enabled = chkWaitTimes.Enabled =
                                    chkOddsPercentage.Enabled = chkUseHedgeWaittime.Enabled = chkUseGreenWaittime.Enabled = spnHedgeWaitTime.Enabled =
                                    spnHedgePlaytime.Enabled = spnHedgePercentage.Enabled = spnGreenWaitTime.Enabled = spnGreenPlaytime.Enabled =
                                    spnGreenPercentage.Enabled = chkNoTrade.Enabled = true;

                    cbxTrigger.SelectedValue = _tradeOutCheck.Trigger;
                    if (_tradeOutCheck.Order == 0)
                    {
                        _tradeOutCheck.Order = 10;
                    }
                    spnCheckOrder.Value = _tradeOutCheck.Order;
                    

                    //Überprüfungsregeln
                    setGoalSumRule(chkCheckGoalSum, spnGoalSumLo, spnGoalSumHi);
                    setPlaytimeRule(chkCheckPlaytime, spnPlaytimeLo, spnPlaytimeHi);
                    setRedCardRule(rbnRCEqual, rbnRCTeamA, rbnRCTeamB, rbnNoRedCard);

                    // Trade Out Settings
                    setTradeOutSettings();

                }
            }
        }

        public ctlTradeOutRuleEditOver(TRADETYPE tradeType) : base(tradeType)
        {
            InitializeComponent();            
            fillTriggerCombobox(cbxTrigger);

            //Alle Controls deaktivieren
            cbxTrigger.Enabled = spnCheckOrder.Enabled = chkCheckPlaytime.Enabled = spnPlaytimeLo.Enabled = spnPlaytimeHi.Enabled =
                chkCheckGoalSum.Enabled = spnGoalSumLo.Enabled = spnGoalSumHi.Enabled = rbnNoRedCard.Enabled = rbnRCEqual.Enabled =
                rbnRCTeamA.Enabled = rbnRCTeamB.Enabled = chkOnlyHedge.Enabled = chkCheckLayOdds.Enabled = chkWaitTimes.Enabled =
                chkOddsPercentage.Enabled = chkUseHedgeWaittime.Enabled = chkUseGreenWaittime.Enabled = spnHedgeWaitTime.Enabled =
                spnHedgePlaytime.Enabled = spnHedgePercentage.Enabled = spnGreenWaitTime.Enabled = spnGreenPlaytime.Enabled =
                spnGreenPercentage.Enabled = chkNoTrade.Enabled = false;
        }

        public ctlTradeOutRuleEditOver()
        {
            InitializeComponent();            
            fillTriggerCombobox(cbxTrigger);

            //Alle Controls deaktivieren
            cbxTrigger.Enabled = spnCheckOrder.Enabled = chkCheckPlaytime.Enabled = spnPlaytimeLo.Enabled = spnPlaytimeHi.Enabled =
                chkCheckGoalSum.Enabled = spnGoalSumLo.Enabled = spnGoalSumHi.Enabled = rbnNoRedCard.Enabled = rbnRCEqual.Enabled =
                rbnRCTeamA.Enabled = rbnRCTeamB.Enabled = chkOnlyHedge.Enabled = chkCheckLayOdds.Enabled = chkWaitTimes.Enabled =
                chkOddsPercentage.Enabled = chkUseHedgeWaittime.Enabled = chkUseGreenWaittime.Enabled = spnHedgeWaitTime.Enabled =
                spnHedgePlaytime.Enabled = spnHedgePercentage.Enabled = spnGreenWaitTime.Enabled = spnGreenPlaytime.Enabled =
                spnGreenPercentage.Enabled = chkNoTrade.Enabled = false;
        }

        protected override void getTradeOutSettings()
        {
            _tradeOutCheck.TradeOutSettings.OnlyHedge         = chkOnlyHedge.Checked;
            _tradeOutCheck.TradeOutSettings.CheckLayOdds      = chkCheckLayOdds.Checked;
            _tradeOutCheck.TradeOutSettings.UseWaitTime       = chkWaitTimes.Checked;
            _tradeOutCheck.TradeOutSettings.UseOddsPercentage = chkOddsPercentage.Checked;
            _tradeOutCheck.TradeOutSettings.UseHedgeWaitTime  = chkUseHedgeWaittime.Checked;
            _tradeOutCheck.TradeOutSettings.UseGreenWaitTime  = chkUseGreenWaittime.Checked;
            _tradeOutCheck.TradeOutSettings.NoTrade           = chkNoTrade.Checked;

            _tradeOutCheck.TradeOutSettings.HedgeWaitTime     = (int)spnHedgeWaitTime.Value;
            _tradeOutCheck.TradeOutSettings.HedgePlaytime     = (int)spnHedgePlaytime.Value;

            _tradeOutCheck.TradeOutSettings.GreenWaitTime     = (int)spnGreenWaitTime.Value;
            _tradeOutCheck.TradeOutSettings.GreenPlaytime     = (int)spnGreenPlaytime.Value;

            _tradeOutCheck.TradeOutSettings.HedgePercentage   = (int)spnHedgePercentage.Value;
            _tradeOutCheck.TradeOutSettings.GreenPercentage   = (int)spnGreenPercentage.Value;
        }

        protected override void setTradeOutSettings()
        {
            chkOnlyHedge.Checked        = _tradeOutCheck.TradeOutSettings.OnlyHedge;
            chkCheckLayOdds.Checked     = _tradeOutCheck.TradeOutSettings.CheckLayOdds;
            chkWaitTimes.Checked        = _tradeOutCheck.TradeOutSettings.UseWaitTime;
            chkOddsPercentage.Checked   = _tradeOutCheck.TradeOutSettings.UseOddsPercentage;
            chkUseHedgeWaittime.Checked = _tradeOutCheck.TradeOutSettings.UseHedgeWaitTime;
            chkUseGreenWaittime.Checked = _tradeOutCheck.TradeOutSettings.UseGreenWaitTime;
            chkNoTrade.Checked          = _tradeOutCheck.TradeOutSettings.NoTrade;

            spnHedgeWaitTime.Value      = _tradeOutCheck.TradeOutSettings.HedgeWaitTime;
            spnHedgePlaytime.Value      = _tradeOutCheck.TradeOutSettings.HedgePlaytime;

            spnGreenWaitTime.Value      = _tradeOutCheck.TradeOutSettings.GreenWaitTime;
            spnGreenPlaytime.Value      = _tradeOutCheck.TradeOutSettings.GreenPlaytime;

            spnHedgePercentage.Value    = _tradeOutCheck.TradeOutSettings.HedgePercentage;
            spnGreenPercentage.Value    = _tradeOutCheck.TradeOutSettings.GreenPercentage;
        }
                               
        private void gbxSettings_SizeChanged(object sender, EventArgs e)
        {
            gbxWaitTimes.Width = gbxSettings.Width - 10;
            gbxOddsPercentage.Width = gbxSettings.Width - 10;
            gbxPlaytime.Width = gbxSettings.Width - 10;
        }

        private void chkCheckPlaytime_CheckedChanged(object sender, EventArgs e)
        {
            if (chkCheckPlaytime.Checked == true)
            {
                spnPlaytimeHi.Enabled = spnPlaytimeLo.Enabled = true;
            }
            else
            {
                spnPlaytimeHi.Enabled = spnPlaytimeLo.Enabled = false;
            }
        }

        private void chkCheckGoalSum_CheckedChanged(object sender, EventArgs e)
        {
            if (chkCheckGoalSum.Checked == true)
            {
                spnGoalSumHi.Enabled = spnGoalSumLo.Enabled = true;
            }
            else
            {
                spnGoalSumHi.Enabled = spnGoalSumLo.Enabled = false;
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            cbxTrigger.Enabled = spnCheckOrder.Enabled = chkCheckPlaytime.Enabled = spnPlaytimeLo.Enabled = spnPlaytimeHi.Enabled =
                chkCheckGoalSum.Enabled = spnGoalSumLo.Enabled = spnGoalSumHi.Enabled = rbnNoRedCard.Enabled = rbnRCEqual.Enabled =
                rbnRCTeamA.Enabled = rbnRCTeamB.Enabled = chkOnlyHedge.Enabled = chkCheckLayOdds.Enabled = chkWaitTimes.Enabled =
                chkOddsPercentage.Enabled = chkUseHedgeWaittime.Enabled = chkUseGreenWaittime.Enabled = spnHedgeWaitTime.Enabled =
                spnHedgePlaytime.Enabled = spnHedgePercentage.Enabled = spnGreenWaitTime.Enabled = spnGreenPlaytime.Enabled =
                spnGreenPercentage.Enabled = chkNoTrade.Enabled = true;

            TTRTradeOutCheck tmp = new TTRTradeOutCheck();
            tmp.Trigger = TRADEOUTTRIGGER.GENERAL;
            tmp.Order = 1;
            tmp.TradeOutSettings.TradeType = this._tradeType;
            this.TradeOutCheck = tmp;
            

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (this.TradeOutCheck == null)
                return;
            /*
            EventHandler<TTRTOElementSaveEventArgs> saveHandler = SaveTOElement;
            if (saveHandler != null)
            {
                saveHandler(this, new TTRTOElementSaveEventArgs(this.TradeOutCheck));
            }*/
            OnSaveTOElement(new TTRTOElementSaveEventArgs(this.TradeOutCheck));
        }

        private void spnPlaytimeLo_ValueChanged(object sender, EventArgs e)
        {
            if (chkCheckPlaytime.Checked)
            {

            }
        }

        private void spnPlaytimeHi_ValueChanged(object sender, EventArgs e)
        {

        }

        private void spnGoalSumLo_ValueChanged(object sender, EventArgs e)
        {

        }

        private void spnGoalSumHi_ValueChanged(object sender, EventArgs e)
        {

        }
    }


}
